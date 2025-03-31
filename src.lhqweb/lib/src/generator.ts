import { isNullOrEmpty, jsonParseOrDefault, tryJsonParse } from './utils';
import { AppError } from './AppError';
import { RootModelElement } from './model/rootModelElement';
import { registerHelpers } from './helpers';
import { type OutputFileData, type OutputInlineData, TemplateRootModel } from './model/templateRootModel';
import { HbsTemplateManager } from './hbsManager';
import { DefaultCodeGenSettings } from './model/modelConst';
import type { CodeGeneratorBasicSettings } from './api/modelTypes';
import type { GeneratedFile, GenerateResult } from './api/types';
import { GeneratorInitialization, IHostEnvironment } from './types';
import { LhqModel } from './api/schemas';
import { validateLhqModel } from './generatorUtils';

export const GeneratorHostDataKeys = Object.freeze({
    namespace: 'namespace',
    fileHeader: 'fileHeader'
});

export class Generator {
    private static _initialized = false;
    private static regexLF = new RegExp('\\r\\n|\\r', 'g');
    private static regexCRLF = new RegExp('(\\r(?!\\n))|((?<!\\r)\\n)', 'g');
    private static hostEnv: IHostEnvironment;

    private _generatedFiles: GeneratedFile[] = [];

    public static initialize(initialization: GeneratorInitialization): void {
        if (!Generator._initialized) {
            if (isNullOrEmpty(initialization)) {
                throw new Error('Generator initialization is required !');
            }

            if (isNullOrEmpty(initialization.hbsTemplates)) {
                throw new Error('Handlebars templates are required (initialization.hbsTemplates) !');
            }

            if (Object.keys(initialization.hbsTemplates).length === 0) {
                throw new Error('Handlebars templates cannot be empty are required (initialization.hbsTemplates) !');
            }

            if (isNullOrEmpty(initialization.hostEnvironment)) {
                throw new Error('Host environment is required (initialization.hostEnvironment) !');
            }

            HbsTemplateManager.init(initialization.hbsTemplates);

            Generator.hostEnv = initialization.hostEnvironment;
            registerHelpers(initialization.hostEnvironment);
            Generator._initialized = true;
        }
    }

    /**
     * Gets the generated content string that should be written to the fileName file.
     * 
     * @param generatedFile - Information about the generated file.
     * @param applyLineEndings - Flag indicating whether to apply line endings to the content or not.
     * @returns The generated content string, without any modifications if applyLineEndings is false, or with line endings applied if applyLineEndings is true.
     */
    public getFileContent(generatedFile: GeneratedFile, applyLineEndings: boolean): string {
        if (!applyLineEndings || generatedFile.content.length === 0) {
            return generatedFile.content;
        }

        return generatedFile.lineEndings === 'LF'
            ? generatedFile.content.replace(Generator.regexLF, '\n')
            : generatedFile.content.replace(Generator.regexCRLF, '\r\n');
    }

    /**
     * Runs code templates for the given input LHQ model.
     * @param model - file *.lhq as deserialized JSON object, not yet validated agains LHQ model schema
     * @param fileName - input LHQ model file name (*.lhq)
     * @param data - external host data as key-value mapping that will be used by code generator templates.
     * @returns generator result.
     */
    public generate(fileName: string, modelData: LhqModel | string, data?: Record<string, unknown> | string): GenerateResult {
        if (!Generator._initialized) {
            throw new AppError('Generator not initialized !');
        }

        if (isNullOrEmpty(fileName)) {
            throw new AppError('Missing input model file name !');
        }

        if (isNullOrEmpty(modelData)) {
            throw new AppError('Missing input modelData !');
        }

        let hostData: Record<string, unknown> = {};
        if (typeof data === 'string') {
            hostData = jsonParseOrDefault(data, {}, true);
        } else if (typeof data === 'object') {
            hostData = data ?? {};
        } else {
            throw new AppError('Invalid host data type (object or string expected) !');
        }

        hostData ??= {};

        //const validation = validateLhqModel(modelData, (msg) => Generator.hostEnv.debugLog(msg));
        const validation = validateLhqModel(modelData);
        //Generator.hostEnv.debugLog(`Validated LHQ model '${fileName}' ... `+ JSON.stringify(validation));
        if (!validation.success) {
            throw new AppError(validation.error ?? `Unable to deserialize or validate LHQ model '${fileName}' !`);
        }

        const model = validation.model as LhqModel;
        const rootModel = new RootModelElement(model);

        const templateId = rootModel.codeGenerator?.templateId ?? '';
        if (isNullOrEmpty(rootModel.codeGenerator) || isNullOrEmpty(templateId)) {
            throw new AppError(`LHQ model '${fileName}' missing code generator template information !`);
        }

        const saveInlineOutputs = (templId: string, inlineOutputs: OutputInlineData[]): void => {
            if (inlineOutputs) {
                inlineOutputs.forEach(inline => {
                    this.addResultFile(templId, inline.content, inline);
                });
            }
        }

        const templateModel = new TemplateRootModel(rootModel, {}, hostData);

        // run handlebars template generator
        templateModel.setCurrentTemplateId(templateId);
        try {
            const templateResult = HbsTemplateManager.runTemplate(templateId, templateModel);

            const mainOutput = templateModel.output;
            this.addResultFile(templateId, templateResult, mainOutput);

            // save inline outputs (of main template) if any
            saveInlineOutputs(templateId, templateModel.inlineOutputs);
        } finally {
            templateModel.setCurrentTemplateId(undefined);
        }

        // process child outputs (if any)
        templateModel.childOutputs.forEach(child => {
            templateModel.setAsChildTemplate(child);

            // run handlebars template generator
            templateModel.setCurrentTemplateId(child.templateId);
            try {
                const templateResult = HbsTemplateManager.runTemplate(child.templateId, templateModel);

                const output = templateModel.output;
                if (isNullOrEmpty(output)) {
                    throw new AppError(`Template '${child.templateId}' missing main output file information (missing 'm-output' helper) !`);
                }

                this.addResultFile(child.templateId, templateResult, output);

                // save inline outputs (of child template) if any
                saveInlineOutputs(child.templateId, templateModel.inlineOutputs);
            } finally {
                templateModel.setCurrentTemplateId(undefined);
            }
        });


        return { generatedFiles: this._generatedFiles };
    }

    private addResultFile(templateId: string, templateResult: string, output: OutputFileData | undefined) {
        if (isNullOrEmpty(output)) {
            throw new AppError(`Template '${templateId}' missing main output file information (missing 'm-output' helper) !`);
        }

        if (isNullOrEmpty(output.fileName)) {
            throw new AppError(`Template '${templateId}' missing main output file name (missing property 'fileName' in 'm-output' helper) !`);
        }

        if (isNullOrEmpty(output.settings)) {
            throw new AppError(`Template '${templateId}' missing main output settings (in 'm-output' helper) !`);
        }

        this.addResultFileInternal(templateResult, output.fileName, output.settings);
    }

    private addResultFileInternal(templateResult: string, fileName: string, settings: CodeGeneratorBasicSettings): void {
        if (settings.Enabled) {
            const genFileName = isNullOrEmpty(settings.OutputFolder) ? fileName : Generator.hostEnv.pathCombine(settings.OutputFolder, fileName);
            const bom = settings.EncodingWithBOM;
            const lineEndings = settings.LineEndings ?? DefaultCodeGenSettings.LineEndings;
            const result: GeneratedFile = { fileName: genFileName, content: templateResult, bom, lineEndings };
            this._generatedFiles.push(result);
        }
    }
}