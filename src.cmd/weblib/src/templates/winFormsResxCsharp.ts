import {CSharpWinFormsGeneratorSettings} from "../types";
import {CSharpResXTemplateBase} from "./csharpResXTemplateBase";

export class WinFormsResxCsharp01Template extends CSharpResXTemplateBase<CSharpWinFormsGeneratorSettings> {
    public static get Id(): string {
        return 'WinFormsResxCsharp01';
    }

    public get csharpTemplateName(): string {
        return WinFormsResxCsharp01Template.Id;
    }
}
