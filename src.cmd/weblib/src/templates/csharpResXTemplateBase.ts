import {CSharpGeneratorSettings, ModelDataNode, ResXGeneratorSettings, TemplateRootModel} from "../types";
import {CodeGeneratorTemplate} from "./codeGeneratorTemplate";
import {HostEnv} from "../hostEnv";
import {isNullOrEmpty} from "../utils";
import {AppError} from "../AppError";

export type CSharpResXTemplateSettings<TCSharpSettings extends CSharpGeneratorSettings> = {
    CSharp: TCSharpSettings;
    ResX: ResXGeneratorSettings;
}

export abstract class CSharpResXTemplateBase<TCSharpSettings extends CSharpGeneratorSettings> extends CodeGeneratorTemplate {
    protected _settings!: CSharpResXTemplateSettings<TCSharpSettings>;

    constructor(handlebarFiles: Record<string, string>) {
        super(handlebarFiles);
    }

    protected abstract get csharpTemplateName(): string;

    protected abstract getRootCsharpClassName(rootModel: TemplateRootModel): string;

    private checkHasNamespaceName(rootModel: TemplateRootModel): void {
        const key = 'namespace';
        if (isNullOrEmpty(rootModel.host[key])) {
            throw new AppError(`Missing value for parameter '${key}'.`,
                `> provide valid path to *.csproj which uses required lhq model\n`+ 
                 `> or provide value for parameter '${key}' in cmd data parameters`);
        }
    }
    
    protected debugLog(msg: string) {
        HostEnv.debugLog(msg);
    }

    public generate(rootModel: TemplateRootModel) {
        const modelVersion = rootModel.model.model.version;
        // if (modelVersion < 2) {
        //     throw new AppError(`Current LHQ file version (${modelVersion}) is not supported! (min version 2 is supported)`);
        // }

        const defaultCompatibleTextEncoding = modelVersion < 2;
        const modelName = rootModel.model.model.name;

        rootModel.extra = rootModel.extra ?? {};

        if (this._settings.CSharp.Enabled.isTrue()) {
            this.checkHasNamespaceName(rootModel);
            rootModel.extra['rootClassName'] = this.getRootCsharpClassName(rootModel);
            const csfileContent = this.compileAndRun(this.csharpTemplateName, rootModel);
            const csFileName = this.prepareFilePath(modelName + '.gen.cs', this._settings.CSharp);
            HostEnv.addResultFile(csFileName, csfileContent);
        }

        if (this._settings.ResX.Enabled.isTrue()) {
            rootModel.extra['useHostWebHtmlEncode'] = isNullOrEmpty(this._settings.ResX.CompatibleTextEncoding)
                ? defaultCompatibleTextEncoding
                : this._settings.ResX.CompatibleTextEncoding.isTrue();

            rootModel.model.languages?.forEach(lang => {
                if (!isNullOrEmpty(lang)) {
                    rootModel.extra['lang'] = lang;
                    const resxfileContent = this.compileAndRun('SharedResx', rootModel);
                    const resxfileName = this.prepareFilePath(`${modelName}.${lang}.resx`, this._settings.ResX);
                    HostEnv.addResultFile(resxfileName, resxfileContent);
                }
            });
        }
    }

    loadSettings(node: ModelDataNode): CSharpResXTemplateSettings<TCSharpSettings> {
        const result: CSharpResXTemplateSettings<TCSharpSettings> = {
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
