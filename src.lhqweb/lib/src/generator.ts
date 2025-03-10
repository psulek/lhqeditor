import Handlebars from "handlebars";

//import { LineEndings, OutputSettings } from './types';
import { isNullOrEmpty, jsonQuery, validateLhqModel } from './utils';
import { GeneratedFile } from './generatedFile';
import { LhqModel, LhqModelCodeGeneratorBasicSettings } from './model/api';
import { AppError } from './AppError';
import { RootModelElement } from './model/rootModelElement';
import { HostEnvironmentDefault, IHostEnvironment } from './hostEnv';
import { getKnownHelpers, registerHelpers } from './helpers';
import { GenerateResult } from './generateResult';
import { TemplateRootModel } from './model/templateRootModel';
import { DefaultLineEndings } from './model/modelConst';

export const DataKeys = Object.freeze({
    Namespace: 'namespace'
});

export type GeneratorSource = {
    model: LhqModel;
    fileName: string;
    // csProjectFileName: string;
    // outDir: string;
};

export class HbsTemplateManager {
    private static _sources: {
        [templateId: string]: string;
    }

    private static _compiled: {
        [templateId: string]: HandlebarsTemplateDelegate;
    }

    public static registerTemplate(templateId: string, handlebarContent: string): void {
        HbsTemplateManager._sources ??= {};
        HbsTemplateManager._sources[templateId] = handlebarContent;
    }

    public static runTemplate(templateId: string, data: unknown): string {
        let compiled: HandlebarsTemplateDelegate;

        HbsTemplateManager._compiled ??= {};
        if (!HbsTemplateManager._compiled.hasOwnProperty(templateId)) {
            if (!HbsTemplateManager._sources.hasOwnProperty(templateId)) {
                throw new AppError(`Template with id '${templateId}' not found !`);
            }

            const source = HbsTemplateManager._sources[templateId];
            compiled = Handlebars.compile(source, { knownHelpers: getKnownHelpers() });

            HbsTemplateManager._compiled[templateId] = compiled;
        } else {
            compiled = HbsTemplateManager._compiled[templateId];
        }

        const result = compiled(data, {
            allowProtoPropertiesByDefault: true,
            allowProtoMethodsByDefault: true,
            allowCallsToHelperMissing: true
        });
        if (result.indexOf('¤') > -1) {
            // NOTE: special tag to remove one tab (decrease indent)
            return result.replace(/\t¤$/gm, '');
        }

        return result;
    }
}

export class Generator {
    private static _initialized = false;

    public static initialize(hostEnv?: IHostEnvironment): void {
        if (!Generator._initialized) {
            const globalObject = typeof window !== 'undefined' ? window : global;
            globalObject.HostEnvironment = hostEnv ?? new HostEnvironmentDefault();
            Generator._initialized = true;

            registerHelpers();
        }
    }


    public static generate(inputModel: GeneratorSource, hostData?: Record<string, unknown>): GenerateResult {
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
        if (isNullOrEmpty(templateId)) {
            throw new AppError(`LHQ model '${fileName}' missing code generator template information !`);
        }
        
        const codeGenSettings = rootModel.codeGenerator?.settings;
        if (isNullOrEmpty(codeGenSettings)) {
            throw new AppError(`LHQ model '${fileName}' missing code generator settings information !`);
        }

        const templateModel = new TemplateRootModel(rootModel, {}, hostData);

        const generatedFiles: GeneratedFile[] = [];

        // run handlebars template generator
        const templateResult = HbsTemplateManager.runTemplate(templateId, templateModel);

        const rootOutputFile = templateModel.rootOutputFile;
        if (isNullOrEmpty(rootOutputFile)) {
            throw new AppError(`LHQ model '${fileName}' missing root output file information (missing 'm-outputFile' helper) !`);
        }

        const genFileName = Generator.prepareFilePath('root output file', rootOutputFile.fileName, rootOutputFile.settings);
        generatedFiles.push(new GeneratedFile(genFileName, templateResult, false, DefaultLineEndings));
        return new GenerateResult(generatedFiles, []);
    }

    private static prepareFilePath(fileDescription: string, fileName: string, settings: LhqModelCodeGeneratorBasicSettings): string {
        if (isNullOrEmpty(fileName)) {
            throw new AppError(`Missing file name for '${fileDescription}' !`);
        }

        return isNullOrEmpty(settings.OutputFolder) ? fileName: HostEnvironment.pathCombine(settings.OutputFolder, fileName);
    }

    // private addResultFile(name: string, content: string, bom: boolean, lineEndings: LineEndings) {
    //     this._generatedFiles.push(new GeneratedFile(name, content, bom, lineEndings));
    // }

    // private addModelGroupSettings(group: string, settings: unknown) {
    //     const json = JSON.stringify(settings);
    //     console.log(`Added model group '${group}' with settings: ` + json);
    // }

    // private pathCombine(path1: string, path2: string): string {
    //     return path.join(path1, path2);
    // }

    // private webHtmlEncode(input: string): string {
    //     return textEncode(input, { mode: 'html' });
    // }

    // private stopwatchStart(): number {
    //     return performance.now();
    // }

    // private stopwatchEnd(start: number): string {
    //     return `${(performance.now() - start).toFixed(2)}ms`;
    // }
}