import {ModelDataNode, OutputSettings, TemplateRootModel} from "../types";
import {isNullOrEmpty} from "../utils";
import {HostEnv} from "../hostEnv";

export interface CodeGeneratorTemplateConstructor {
    new(handlebarFiles: Record<string, string>): CodeGeneratorTemplate;
}

export abstract class CodeGeneratorTemplate {
    private handlebarFiles: Record<string, string>;

    constructor(handlebarFiles: Record<string, string>) {
        this.handlebarFiles = handlebarFiles;
    }

    protected getHandlebarFile(name: string): string {
        const file = this.handlebarFiles[name];
        if (file === undefined || file === '') {
            throw new Error(`Handlebar file with name '${name}' not found !`);
        }
        return file;
    }

    protected prepareFilePath(fileName: string, templateRootModel: TemplateRootModel, outputSettings: OutputSettings): string {
        const rootOutputFolder = templateRootModel.host?.['rootOutputFolder'] as string;
        const outputFolder = outputSettings.OutputFolder;

        if (!isNullOrEmpty(outputFolder)) {
            return isNullOrEmpty(rootOutputFolder)
                ? HostEnv.pathCombine(outputFolder, fileName)
                : HostEnv.pathCombine(HostEnv.pathCombine(rootOutputFolder, outputFolder), fileName);
        }

        return isNullOrEmpty(rootOutputFolder)
            ?  fileName
            : HostEnv.pathCombine(rootOutputFolder, fileName);
    }

    public abstract loadSettings(node: ModelDataNode): Object;

    public abstract generate(rootModel: TemplateRootModel, handlebarFiles: Record<string, string>): void;

    protected compile(handlebarsTemplate: string, data: unknown): string {
        // @ts-ignore
        const compiled = Handlebars.compile(handlebarsTemplate);
        return compiled(data);
    }
}


