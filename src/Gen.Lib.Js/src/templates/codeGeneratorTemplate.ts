import {GeneratorSettings, LineEndings, ModelDataNode, OutputSettings, TemplateRootModel} from "../types";
import {copyObject, isNullOrEmpty, valueOrDefault} from "../utils";
import {HostEnv} from "../hostEnv";
import {clearHelpersContext, debugHelpersTimeTaken, getKnownHelpers} from "../helpers";

export interface CodeGeneratorTemplateConstructor {
    new(handlebarFiles: Record<string, string>): CodeGeneratorTemplate;
}

type HbsCompiledType = (data: unknown) => string;

export abstract class CodeGeneratorTemplate {
    private readonly handlebarFiles: Record<string, string>;
    private lastCompiledTemplate: { templateFileName: string, compiled: HbsCompiledType } | undefined;

    constructor(handlebarFiles: Record<string, string>) {
        this.handlebarFiles = handlebarFiles;
    }

    private getHandlebarFile(templateName: string): string {
        const file = this.handlebarFiles[templateName];
        if (file === undefined || file === '') {
            throw new Error(`Handlebar file with name '${templateName}' not found !`);
        }
        return file;
    }

    protected prepareFilePath(fileName: string, outputSettings: OutputSettings): string {
        const outputFolder = outputSettings.OutputFolder;
        return isNullOrEmpty(outputFolder) ? fileName : HostEnv.pathCombine(outputFolder, fileName);
    }

    public abstract loadSettings(node: ModelDataNode): Object;

    public abstract generate(rootModel: TemplateRootModel): void;
    
    protected addResultFile(name: string, content: string, outputSettings: OutputSettings) {
        HostEnv.addResultFile(name, content, outputSettings.EncodingWithBOM ?? false,
            outputSettings.LineEndings ?? 'lf');
    }
    
    protected setDefaults(outputSettings: GeneratorSettings): void {
        outputSettings.EncodingWithBOM = valueOrDefault(outputSettings.EncodingWithBOM, false);
        outputSettings.LineEndings = valueOrDefault(outputSettings.LineEndings, 'lf');
        outputSettings.Enabled = outputSettings.Enabled ?? true.toString();
    }
    
    protected addModelGroupSettings<T, K extends keyof T>(group: string, settings: T, keysToSkip?: K[]) {
        let obj = settings as unknown;
        if (!isNullOrEmpty(keysToSkip) && keysToSkip.length > 0) {
            obj = copyObject(settings as any, keysToSkip);
        }
        
        HostEnv.addModelGroupSettings(group, obj);
    }

    protected compileAndRun(templateFileName: string, data: unknown): string {
        let compiled: HbsCompiledType | undefined;

        if (this.lastCompiledTemplate === undefined || this.lastCompiledTemplate.templateFileName.toLowerCase() !== templateFileName.toLowerCase()) {
            const handlebarsTemplate = this.getHandlebarFile(templateFileName);
            const options = {knownHelpers: getKnownHelpers()};
            // @ts-ignore
            compiled = Handlebars.compile(handlebarsTemplate, options) as HbsCompiledType;

            this.lastCompiledTemplate = {
                templateFileName: templateFileName,
                compiled: compiled
            };
        } else {
            compiled = this.lastCompiledTemplate.compiled;
        }

        if (isNullOrEmpty(compiled)) {
            throw new Error(`Template '${templateFileName}' was not found !`);
        }

        clearHelpersContext();

        let result = compiled(data);
        result = result.replace(/\t¤$/gm, "");
        return result;
    }
}


