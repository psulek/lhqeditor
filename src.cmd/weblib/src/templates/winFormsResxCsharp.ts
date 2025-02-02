import {CSharpWinFormsGeneratorSettings, TemplateRootModel} from "../types";
import {CSharpResXTemplateBase} from "./csharpResXTemplateBase";

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
}
