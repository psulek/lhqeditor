import {CSharpGeneratorSettings, TemplateRootModel} from "../types";
import {CSharpResXTemplateBase} from "./csharpResXTemplateBase";

export class WpfResxCsharp01Template extends CSharpResXTemplateBase<CSharpGeneratorSettings> {
    public static get Id(): string {
        return 'WpfResxCsharp01';
    }

    public get csharpTemplateName(): string {
        return WpfResxCsharp01Template.Id;
    }
    
    public getRootCsharpClassName(rootModel: TemplateRootModel): string {
        return rootModel.model.model.name + 'Context';
    }
}