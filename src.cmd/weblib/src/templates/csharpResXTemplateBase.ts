import {CSharpGeneratorSettings, ModelDataNode, ResXGeneratorSettings, TemplateRootModel} from "../types";
import {CodeGeneratorTemplate} from "./codeGeneratorTemplate";
import {HostEnv} from "../hostEnv";
import {isNullOrEmpty} from "../utils";

type Settings<TCSharpSettings extends CSharpGeneratorSettings> = {
    CSharp: TCSharpSettings;
    ResX: ResXGeneratorSettings;
}

export abstract class CSharpResXTemplateBase<TCSharpSettings extends CSharpGeneratorSettings> extends CodeGeneratorTemplate {
    private _settings!: Settings<TCSharpSettings>;

    constructor(handlebarFiles: Record<string, string>) {
        super(handlebarFiles);
    }

    protected abstract get csharpTemplateName(): string;

    public generate(rootModel: TemplateRootModel) {
        const modelName = rootModel.model.model.name;

        if (this._settings.CSharp.Enabled) {
            rootModel.extra = {};
            const csharpTemplateFile = this.getHandlebarFile(this.csharpTemplateName);
            const csfileContent = this.compile(csharpTemplateFile, rootModel);
            const csFileName = this.prepareFilePath(modelName + '.gen.cs', this._settings.CSharp);
            HostEnv.addResultFile(csFileName, csfileContent);
        }

        if (this._settings.ResX.Enabled) {
            const resxTemplateFile = this.getHandlebarFile('resx');
            rootModel.extra = {};
            
            rootModel.model.languages?.forEach(lang => {
                if (!isNullOrEmpty(lang)) {
                    rootModel.extra['lang'] = lang;
                    const resxfileContent = this.compile(resxTemplateFile, rootModel);
                    const resxfileName = this.prepareFilePath(`${modelName}.${lang}.resx`, this._settings.ResX);
                    HostEnv.addResultFile(resxfileName, resxfileContent);
                }
            });
        }
    }

    loadSettings(node: ModelDataNode): Settings<TCSharpSettings> {
        const result: Settings<TCSharpSettings> = {
            CSharp: undefined!,
            ResX: undefined!
        };

        node.childs?.forEach(x => {
            const attrs = x.attrs as unknown;
            switch (x.name) {
                case 'CSharp':
                    result.CSharp = attrs as TCSharpSettings;
                    break;
                case 'ResX':
                    result.ResX = attrs as ResXGeneratorSettings;
                    break;
            }
        });

        if (result.CSharp === undefined) {
            throw new Error('CSharp settings not found !');
        }

        if (result.ResX === undefined) {
            throw new Error('ResX settings not found !');
        }

        result.CSharp.Enabled = result.CSharp.Enabled ?? true;
        result.ResX.Enabled = result.ResX.Enabled ?? true;

        this._settings = result;
        return result;
    }
}
