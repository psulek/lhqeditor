import {LhqModelType, ModelDataNode, TemplateRootModel} from "./types";
import {registerHelpers} from "./helpers";
import {TypescriptJson01Template} from "./templates/typescriptJson";
import {CodeGeneratorTemplate, CodeGeneratorTemplateConstructor} from "./templates/codeGeneratorTemplate";
import {NetCoreResxCsharp01Template} from "./templates/netCoreResxCsharp";
import {NetFwResxCsharp01Template} from "./templates/netFwResxCsharp";
import {WinFormsResxCsharp01Template} from "./templates/winFormsResxCsharp";
import {WpfResxCsharp01Template} from "./templates/wpfResxCsharp";

const CodeGenUID = 'b40c8a1d-23b7-4f78-991b-c24898596dd2';

export class TemplateManager {
    private static handlebarFiles: Record<string, string>;
    
    private static generators: Record<string, CodeGeneratorTemplateConstructor> = {
        [TypescriptJson01Template.Id]: TypescriptJson01Template,
        [NetCoreResxCsharp01Template.Id]: NetCoreResxCsharp01Template,
        [NetFwResxCsharp01Template.Id]: NetFwResxCsharp01Template,
        [WinFormsResxCsharp01Template.Id]: WinFormsResxCsharp01Template,
        [WpfResxCsharp01Template.Id]: WpfResxCsharp01Template
    };
    
    public static intialize(handlebarFiles: string): void {
        TemplateManager.handlebarFiles = JSON.parse(handlebarFiles) as Record<string, string>;
        registerHelpers();
    }
    
    public static runTemplate(lhqModelJson: string, hostData: string): void {
        const lhqModel = JSON.parse(lhqModelJson) as LhqModelType;
        const {template, templateId, settingsNode} = TemplateManager.loadTemplate(lhqModel as LhqModelType)
        let settings = template.loadSettings(settingsNode);
        let host = {};
        if (hostData) {
            host = JSON.parse(hostData) as Record<string, unknown>;
        }
        const rootModel: TemplateRootModel = {
            model: lhqModel,
            settings: settings,
            host: host
        };
        
        template.generate(rootModel, TemplateManager.handlebarFiles);
    }

    private static loadTemplate(model: LhqModelType): {
        template: CodeGeneratorTemplate,
        templateId: string,
        settingsNode: ModelDataNode
    } {
        let template: CodeGeneratorTemplate | undefined = undefined;
        let templateId = '';
        let node = model.metadatas?.childs?.find(x => x.name === 'metadata' && x.attrs?.['descriptorUID'] === CodeGenUID);
        if (node) {
            node = node.childs?.find(x => x.name === 'content' && x.attrs?.['templateId'] !== undefined);
            if (node) {
                templateId = node?.attrs['templateId'];
                node = node.childs.find(x => x.name === 'Settings' && x.childs?.length > 0);
            }
        }

        if (node && templateId !== undefined && template !== '') {
            // const handlebarsTemplate = TemplateManager.handlebarFiles[templateId];
            // if (handlebarsTemplate === undefined || handlebarsTemplate === '') {
            //     throw new Error(`Handlebar file for template '${templateId}' not found !`);
            // }
            
            const ctor = TemplateManager.generators[templateId];
            template = (ctor && new ctor(TemplateManager.handlebarFiles)) || undefined;
            
            return {template, templateId, settingsNode: node};
        }

        throw new Error(`Template '${templateId}' not found !`);
    }
}