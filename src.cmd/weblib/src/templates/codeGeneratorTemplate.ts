import {ModelDataNode, OutputSettings, TemplateRootModel} from "../types";
import {isNullOrEmpty} from "../utils";
import {HostEnv} from "../hostEnv";
import {clearHelpersContext, getKnownHelpers} from "../helpers";

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


