// import {
//     CSharpGeneratorSettings,
//     GeneratorSettings,
//     ModelDataNode,
//     ResXGeneratorSettings,
//     TemplateRootModel
// } from "../types";
// import {CodeGeneratorTemplate} from "./codeGeneratorTemplate";
// import {HostEnv} from "../hostEnv";
// import {isNullOrEmpty, valueOrDefault} from "../utils";
// import {AppError} from "../AppError";

// export type CSharpResXTemplateSettings<TCSharpSettings extends CSharpGeneratorSettings> = {
//     CSharp: TCSharpSettings;
//     ResX: ResXGeneratorSettings;
// }

// export abstract class CSharpResXTemplateBase<TCSharpSettings extends CSharpGeneratorSettings> extends CodeGeneratorTemplate {
//     protected _settings!: CSharpResXTemplateSettings<TCSharpSettings>;
//     private _defaultCompatibleTextEncoding = true;

//     constructor(handlebarFiles: Record<string, string>) {
//         super(handlebarFiles);
//     }

//     protected abstract get csharpTemplateName(): string;

//     protected abstract getRootCsharpClassName(rootModel: TemplateRootModel): string;

//     private checkHasNamespaceName(rootModel: TemplateRootModel): void {
//         const key = 'namespace';
//         if (isNullOrEmpty(rootModel.host[key])) {
//             throw new AppError(`Missing value for parameter '${key}'.\n` +
//                 `> provide valid path to *.csproj which uses required lhq model\n`+ 
//                  `> or provide value for parameter '${key}' in cmd data parameters`);
//         }
//     }
    
//     protected debugLog(msg: string) {
//         HostEnv.debugLog(msg);
//     }

//     protected setDefaults(outputSettings: GeneratorSettings, settingsName: string): void {
//         super.setDefaults(outputSettings, settingsName);
        
//         if (settingsName === 'CSharp') {
//         }
//         else if (settingsName === 'ResX') {
//             const resx = outputSettings as ResXGeneratorSettings;
//             // @ts-ignore
//             resx.CompatibleTextEncoding = valueOrDefault(resx.CompatibleTextEncoding,
//                 this._defaultCompatibleTextEncoding.toString());
//         }
//     }

//     public generate(rootModel: TemplateRootModel) {
//         //const modelVersion = rootModel.model.model.version;
//         // if (modelVersion < 2) {
//         //     throw new AppError(`Current LHQ file version (${modelVersion}) is not supported! (min version 2 is supported)`);
//         // }

//         const modelName = rootModel.model.model.name;

//         rootModel.extra = rootModel.extra ?? {};

//         if (this._settings.CSharp.Enabled.isTrue()) {
//             this.checkHasNamespaceName(rootModel);
//             rootModel.extra['rootClassName'] = this.getRootCsharpClassName(rootModel);
//             const csfileContent = this.compileAndRun(this.csharpTemplateName, rootModel);
//             const csFileName = this.prepareFilePath(modelName + '.gen.cs', this._settings.CSharp);
//             this.addResultFile(csFileName, csfileContent, this._settings.CSharp);
//             this.addModelGroupSettings('CSharp', this._settings.CSharp, ['Enabled']);
//         }

//         if (this._settings.ResX.Enabled.isTrue()) {
//             rootModel.extra['useHostWebHtmlEncode'] = this._settings.ResX.CompatibleTextEncoding.isTrue();
            
//             this.addModelGroupSettings('ResX', this._settings.ResX, ['Enabled']);
            
//             rootModel.model.languages?.forEach(lang => {
//                 if (!isNullOrEmpty(lang)) {
//                     rootModel.extra['lang'] = lang;
//                     const resxfileContent = this.compileAndRun('SharedResx', rootModel);
//                     const resxfileName = this.prepareFilePath(`${modelName}.${lang}.resx`, this._settings.ResX);
//                     this.addResultFile(resxfileName, resxfileContent, this._settings.ResX);
//                 }
//             });
//         }
//     }

//     loadSettings(node: ModelDataNode): CSharpResXTemplateSettings<TCSharpSettings> {
//         const result: CSharpResXTemplateSettings<TCSharpSettings> = {
//             CSharp: undefined!,
//             ResX: undefined!
//         };

//         node.childs?.forEach(x => {
//             const attrs = x.attrs as unknown;
//             switch (x.name) {
//                 case 'CSharp':
//                     result.CSharp = attrs as TCSharpSettings;
//                     break;
//                 case 'ResX':
//                     result.ResX = attrs as ResXGeneratorSettings;
//                     break;
//             }
//         });

//         if (result.CSharp === undefined) {
//             throw new Error('CSharp settings not found !');
//         }

//         if (result.ResX === undefined) {
//             throw new Error('ResX settings not found !');
//         }

//         // result.CSharp.Enabled = result.CSharp.Enabled ?? true.toString();
//         // result.ResX.Enabled = result.ResX.Enabled ?? true.toString();

//         this.setDefaults(result.CSharp, 'CSharp');
//         this.setDefaults(result.ResX, 'ResX');

//         // @ts-ignore
//         // result.ResX.CompatibleTextEncoding = valueOrDefault(result.ResX.CompatibleTextEncoding,
//         //     this._defaultCompatibleTextEncoding.toString());

//         this._settings = result;
//         return result;
//     }
// }
