import { isNullOrEmpty, validateLhqModel } from './utils';
import { AppError } from './AppError';
import { RootModelElement } from './model/rootModelElement';
import { HostEnvironmentDefault, IHostEnvironment } from './hostEnv';
import { registerHelpers } from './helpers';
import { TemplateRootModel } from './model/templateRootModel';
import { HbsTemplateManager } from './hbsManager';
import { DefaultCodeGenSettings } from './model/modelConst';
import { GeneratedFile, GeneratorSource, GenerateResult } from './types';
import { CodeGeneratorBasicSettings } from './model/api/types';

export const DataKeys = Object.freeze({
    Namespace: 'namespace'
});

export class Generator {
    private static _initialized = false;
    private static regexLF = new RegExp("\\r\\n|\\r", "g");
    private static regexCRLF = new RegExp("(\\r(?!\\n))|((?<!\\r)\\n)", "g");

    private _generatedFiles: GeneratedFile[] = [];

    public static initialize(hostEnv?: IHostEnvironment): void {
        if (!Generator._initialized) {
            const globalObject = typeof window !== 'undefined' ? window : global;
            globalObject.HostEnvironment = hostEnv ?? new HostEnvironmentDefault();
            Generator._initialized = true;

            registerHelpers();
        }
    }

    /**
     * Gets the generated content string that should be written to the fileName file.
     * 
     * @param generatedFile Information about the generated file.
     * @param applyLineEndings Flag indicating whether to apply line endings to the content or not.
     * @returns The generated content string, without any modifications if applyLineEndings is false, or with line endings applied if applyLineEndings is true.
     */
    public getFileContent(generatedFile: GeneratedFile, applyLineEndings: boolean): string {
        if (!applyLineEndings || generatedFile.content.length === 0) {
            return generatedFile.content;
        }

        return generatedFile.lineEndings === 'LF'
            ? generatedFile.content.replace(Generator.regexLF, "\n")
            : generatedFile.content.replace(Generator.regexCRLF, "\r\n");
    }

    /**
     * Runs code templates for the given input LHQ model.
     * @param inputModel input LHQ model
     * @param hostData external host data
     * @returns generated result.
     */
    public generate(inputModel: GeneratorSource, hostData?: Record<string, unknown>): GenerateResult {
        if (!Generator._initialized) {
            throw new AppError('Generator not initialized !');
        }

        if (isNullOrEmpty(inputModel)) {
            throw new AppError('Missing input model data !');
        }

        const { model, fileName } = inputModel;

        if (isNullOrEmpty(fileName)) {
            throw new AppError('Missing input model file name !');
        }

        if (isNullOrEmpty(model)) {
            throw new AppError('Missing input model !');
        }

        if (!hostData) {
            hostData = {};
        }

        //const rootNamespace = valueOrDefault(hostData[DataKeys.Namespace], '');

        const validation = validateLhqModel(model);
        if (!validation.success) {
            throw new AppError(validation.error ?? `Unable to deserialize LHQ model '${fileName}' !`);
        }

        const rootModel = new RootModelElement(model);

        const templateId = rootModel.codeGenerator?.templateId ?? '';
        if (isNullOrEmpty(rootModel.codeGenerator) || isNullOrEmpty(templateId)) {
            throw new AppError(`LHQ model '${fileName}' missing code generator template information !`);
        }

        const templateModel = new TemplateRootModel(rootModel, {}, hostData);

        // run handlebars template generator
        const templateResult = HbsTemplateManager.runTemplate(templateId, templateModel);

        const mainOutput = templateModel.output;
        if (isNullOrEmpty(mainOutput)) {
            throw new AppError(`Template '${templateId}' missing main output file information (missing 'm-output' helper) !`);
        }

        this.addResultFile('main output file', templateResult, mainOutput.fileName, mainOutput.settings);

        // process child outputs (if any)
        templateModel.childOutputs.forEach(child => {
            templateModel.setAsChildTemplate(child);

            // run handlebars template generator
            const templateResult = HbsTemplateManager.runTemplate(child.templateId, templateModel);

            const output = templateModel.output;
            if (isNullOrEmpty(output)) {
                throw new AppError(`Template '${child.templateId}' missing main output file information (missing 'm-output' helper) !`);
            }

            this.addResultFile('child output file', templateResult, output.fileName, output.settings);
        });


        return { generatedFiles: this._generatedFiles, modelGroupSettings: [] };
    }

    private addResultFile(fileDescription: string, templateResult: string, fileName: string,
        settings: CodeGeneratorBasicSettings): void {

        if (isNullOrEmpty(fileName)) {
            throw new AppError(`Missing file name for '${fileDescription}' !`);
        }

        if (settings.Enabled) {
            const genFileName = isNullOrEmpty(settings.OutputFolder) ? fileName : HostEnvironment.pathCombine(settings.OutputFolder, fileName);
            const bom = settings.EncodingWithBOM;
            const lineEndings = settings.LineEndings ?? DefaultCodeGenSettings.LineEndings;
            const result: GeneratedFile = { fileName: genFileName, content: templateResult, bom, lineEndings };
            this._generatedFiles.push(result);
        }
    }
}