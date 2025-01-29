import {JsonGeneratorSettings, ModelDataNode, TemplateRootModel, TypescriptGeneratorSettings} from '../types';
import {CodeGeneratorTemplate} from "./codeGeneratorTemplate";
import {HostEnv} from "../hostEnv";

type Settings = { Typescript: TypescriptGeneratorSettings, Json: JsonGeneratorSettings };

export class TypescriptJson01Template extends CodeGeneratorTemplate {
    private _settings!: Settings;
    
    generate(rootModel: TemplateRootModel) {
        const handlebarFile = this.getHandlebarFile(TypescriptJson01Template.Id);
        const tsFileContent = this.compile(handlebarFile, rootModel);
        // const csFileName = this.prepareFilePath(modelName + '.gen.cs', rootModel, this._settings.CSharp);
        // HostEnv.addResultFile(csFileName, csfileContent);

    }

    public loadSettings(node: ModelDataNode): Settings {
        const result: Settings = { Typescript: undefined!, Json: undefined! };

        node.childs?.forEach(x => {
            const attrs = x.attrs as unknown;
            switch (x.name) {
                case 'Typescript':
                    result.Typescript = attrs as TypescriptGeneratorSettings;
                    break;
                case 'Json':
                    result.Json = attrs as JsonGeneratorSettings;
                    break;
            }
        });

        if (result.Typescript === undefined) {
            throw new Error('Typescript settings not found !');
        }

        if (result.Json === undefined) {
            throw new Error('Json settings not found !');
        }

        this._settings = result;
        return result;
    }

    public static get Id(): string {
        return 'TypescriptJson01';
    }
}
