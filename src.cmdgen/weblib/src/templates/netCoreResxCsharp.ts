import {CSharpGeneratorSettings} from "../types";
import {CSharpResXTemplateBase} from "./csharpResXTemplateBase";

export class NetCoreResxCsharp01Template extends CSharpResXTemplateBase<CSharpGeneratorSettings> {
    public static get Id(): string {
        return 'NetCoreResxCsharp01';
    }
    
    public get csharpTemplateName(): string {
        return NetCoreResxCsharp01Template.Id;
    }
}
