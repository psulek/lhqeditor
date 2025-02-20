import {JsonGeneratorSettings, ModelDataNode, TemplateRootModel, TypescriptGeneratorSettings} from '../types';
import {CodeGeneratorTemplate} from "./codeGeneratorTemplate";
import {HostEnv} from "../hostEnv";
import {isNullOrEmpty, sortBy, valueOrDefault} from "../utils";

type Settings = { Typescript: TypescriptGeneratorSettings, Json: JsonGeneratorSettings };

export class TypescriptJson01Template extends CodeGeneratorTemplate {
    private _settings!: Settings;

    generate(rootModel: TemplateRootModel) {
        const model = rootModel.model.model;
        const modelName = model.name;

        if (this._settings.Typescript.Enabled.isTrue()) {
            const dtsFileContent = this.compileAndRun(TypescriptJson01Template.Id, rootModel);
            const dtsFileName = this.prepareFilePath(modelName + '.d.ts', this._settings.Typescript);
            this.addResultFile(dtsFileName, dtsFileContent, this._settings.Typescript);
            this.addModelGroupSettings('Typescript', this._settings.Typescript, ['Enabled']);
        }

        if (this._settings.Json.Enabled.isTrue()) {
            //const metadataFileNameSuffix = valueOrDefault(this._settings.Json.MetadataFileNameSuffix, 'metadata');
            const metadataFileNameSuffix = this._settings.Json.MetadataFileNameSuffix;
            const metadataObj = {
                default: model.primaryLanguage,
                languages: sortBy(rootModel.model.languages, undefined, 'asc')
            };
            const metadataContent = JSON.stringify(metadataObj, null, '\t') + '\n';

            const metadataFileName = this.prepareFilePath(`${modelName}-${metadataFileNameSuffix}.json`, this._settings.Json);
            this.addResultFile(metadataFileName, metadataContent, this._settings.Json);
            this.addModelGroupSettings('Json', this._settings.Json, ['Enabled']);

            rootModel.extra = {};

            const writeEmptyValues = this._settings.Json.WriteEmptyValues.isTrue();
            const allFilesHasLangInName = this._settings.Json.CultureCodeInFileNameForPrimaryLanguage.isTrue();
            rootModel.model.languages?.forEach(lang => {
                if (!isNullOrEmpty(lang)) {
                    const isPrimary = model.primaryLanguage === lang;
                    const langName = !isPrimary || allFilesHasLangInName ? `.${lang}` : '';
                    rootModel.extra['lang'] = lang;
                    rootModel.extra['writeEmptyValues'] = writeEmptyValues;
                    const jsonFileContent = this.compileAndRun('JsonPerLanguage', rootModel);
                    const jsonfileName = this.prepareFilePath(`${modelName}${langName}.json`, this._settings.Json);
                    this.addResultFile(jsonfileName, jsonFileContent, this._settings.Json);
                }
            });
        }
    }

    public loadSettings(node: ModelDataNode): Settings {
        const result: Settings = {Typescript: undefined!, Json: undefined!};

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

        //result.Typescript.Enabled = result.Typescript.Enabled ?? true.toString();
        this.setDefaults(result.Typescript);
               
        //result.Json.Enabled = result.Json.Enabled ?? true.toString();
        this.setDefaults(result.Json);
        result.Json.MetadataFileNameSuffix = valueOrDefault(result.Json.MetadataFileNameSuffix, 'metadata');

        this._settings = result;
        return result;
    }

    public static get Id(): string {
        return 'TypescriptJson01';
    }
}
