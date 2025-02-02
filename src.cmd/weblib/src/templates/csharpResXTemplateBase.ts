import {CSharpGeneratorSettings, LhqModelType, ModelDataNode, ResXGeneratorSettings, TemplateRootModel} from "../types";
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
    
    protected abstract getRootCsharpClassName(rootModel: TemplateRootModel): string;

    public generate(rootModel: TemplateRootModel) {
        const modelVersion = rootModel.model.model.version;
        // if (modelVersion < 2) {
        //     throw new AppError(`Current LHQ file version (${modelVersion}) is not supported! (min version 2 is supported)`);
        // }

        const defaultCompatibleTextEncoding = modelVersion < 2;
        const modelName = rootModel.model.model.name;

        if (this._settings.CSharp.Enabled.isTrue()) {
            rootModel.extra = {};
            rootModel.extra['rootClassName'] = this.getRootCsharpClassName(rootModel);
            
            const csharpTemplateFile = this.getHandlebarFile(this.csharpTemplateName);
            const csfileContent = this.compile(csharpTemplateFile, rootModel);
            const csFileName = this.prepareFilePath(modelName + '.gen.cs', this._settings.CSharp);
            HostEnv.addResultFile(csFileName, csfileContent);
        }

        if (this._settings.ResX.Enabled.isTrue()) {
            const resxTemplateFile = this.getHandlebarFile('resx');
            rootModel.extra = {};
            rootModel.extra['useHostWebHtmlEncode'] = isNullOrEmpty(this._settings.ResX.CompatibleTextEncoding)
                ? defaultCompatibleTextEncoding
                : this._settings.ResX.CompatibleTextEncoding.isTrue();
            
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

        result.CSharp.Enabled = result.CSharp.Enabled ?? true.toString();
        result.ResX.Enabled = result.ResX.Enabled ?? true.toString();
        
        this._settings = result;
        return result;
    }
}
