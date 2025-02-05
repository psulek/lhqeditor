import {CSharpGeneratorSettings, TemplateRootModel} from "../types";
import {CSharpResXTemplateBase} from "./csharpResXTemplateBase";

export class NetFwResxCsharp01Template extends CSharpResXTemplateBase<CSharpGeneratorSettings> {
    public static get Id(): string {
        //return 'NetFwResxCsharp01';
        return 'WpfResxCsharp01'; // NetFwResxCsharp01 is same as WpfResxCsharp01!
    }

    public get csharpTemplateName(): string {
        return NetFwResxCsharp01Template.Id;
    }

    public getRootCsharpClassName(rootModel: TemplateRootModel): string {
        return rootModel.model.model.name + 'Context';
    }
}
