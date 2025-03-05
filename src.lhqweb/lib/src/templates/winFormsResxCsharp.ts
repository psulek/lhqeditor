import {CSharpWinFormsGeneratorSettings, ModelDataNode, TemplateRootModel} from "../types";
import {CSharpResXTemplateBase, CSharpResXTemplateSettings} from "./csharpResXTemplateBase";
import {isNullOrEmpty} from "../utils";
import {HostEnv} from "../hostEnv";

export class WinFormsResxCsharp01Template extends CSharpResXTemplateBase<CSharpWinFormsGeneratorSettings> {
    public static get Id(): string {
        return 'WinFormsResxCsharp01';
    }

    public get csharpTemplateName(): string {
        return WinFormsResxCsharp01Template.Id;
    }

    public getRootCsharpClassName(rootModel: TemplateRootModel): string {
        return rootModel.model.model.name + 'Context';
    }

    loadSettings(node: ModelDataNode): CSharpResXTemplateSettings<CSharpWinFormsGeneratorSettings> {
        const settings = super.loadSettings(node);

        settings.CSharp.ParamsMethodsSuffix = settings.CSharp.ParamsMethodsSuffix ?? 'WithParams';
        settings.CSharp.GenerateParamsMethods = settings.CSharp.GenerateParamsMethods ?? true.toString();
        settings.CSharp.MissingTranslationFallbackToPrimary = settings.CSharp.MissingTranslationFallbackToPrimary ?? false.toString();
        
        return settings;
    }

    generate(rootModel: TemplateRootModel) {
        if (this._settings.CSharp.Enabled.isTrue()) {
            rootModel.extra = rootModel.extra ?? {};
            const generateParamsMethods = this._settings.CSharp.GenerateParamsMethods.isTrue();
            rootModel.extra['generateParamsMethods'] = generateParamsMethods;
            rootModel.extra['paramsMethodsSuffix'] = (generateParamsMethods ? (this._settings.CSharp.ParamsMethodsSuffix ?? 'WithParams') : '');
            const bindableFileName = this.prepareFilePath('BindableObject.gen.cs', this._settings.CSharp);
            const bindableContent = this.compileAndRun('WinFormsBindableObject', rootModel);
            this.addResultFile(bindableFileName, bindableContent, this._settings.CSharp);
        }

        super.generate(rootModel);
    }
}
