import {ModelDataNode, OutputSettings, TemplateRootModel} from "../types";
import {isNullOrEmpty} from "../utils";
import {HostEnv} from "../hostEnv";

export interface CodeGeneratorTemplateConstructor {
    new(handlebarFiles: Record<string, string>): CodeGeneratorTemplate;
}

export class HostDataKeys {
    //public static get outDir(): string { return 'outDir'; };
    public static get namespace(): string {
        return 'namespace';
    };
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

    protected prepareFilePath(fileName: string, outputSettings: OutputSettings): string {
        const outputFolder = outputSettings.OutputFolder;
        return isNullOrEmpty(outputFolder) ? fileName : HostEnv.pathCombine(outputFolder, fileName);
    }

    public abstract loadSettings(node: ModelDataNode): Object;

    public abstract generate(rootModel: TemplateRootModel): void;

    protected compile(handlebarsTemplate: string, data: unknown): string {
        // @ts-ignore
        const compiled = Handlebars.compile(handlebarsTemplate);
        let result = compiled(data) as string;
        result = result.replace(/\t¤$/gm, "");
        //result = result.replace(/\t©$/gm, "");
        
        // let err = false;
        // // @ts-ignore
        // result.replace(/^[\t_]*?(\t_)$/gm, function(match, group) {
        //    
        // });
        
        return result;
    }
}


