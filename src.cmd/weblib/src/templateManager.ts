import {
    LhqCategoriesAndResourcesType,
    LhqModelCategoriesCollectionType,
    LhqModelCategoryType, LhqModelCategoryWithNameType, LhqModelResourcesCollectionType, LhqModelResourceWithNameType,
    LhqModelType,
    ModelDataNode,
    TemplateRootModel
} from "./types";
import {registerHelpers} from "./helpers";
import {TypescriptJson01Template} from "./templates/typescriptJson";
import {CodeGeneratorTemplate, CodeGeneratorTemplateConstructor} from "./templates/codeGeneratorTemplate";
import {NetCoreResxCsharp01Template} from "./templates/netCoreResxCsharp";
import {NetFwResxCsharp01Template} from "./templates/netFwResxCsharp";
import {WinFormsResxCsharp01Template} from "./templates/winFormsResxCsharp";
import {WpfResxCsharp01Template} from "./templates/wpfResxCsharp";
import {arraySort, arraySortBy, sortBy} from "./utils";

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
        let lhqModel = JSON.parse(lhqModelJson) as LhqModelType;
        if (lhqModel) {
            lhqModel = TemplateManager.getCategoriesAndResourcesArray(lhqModel);

            const {template, templateId, settingsNode} = TemplateManager.loadTemplate(lhqModel as LhqModelType);
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
        } else {
            throw new Error(`Unable to deserialize LHQ model !`);
        }
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

    private static getCategoriesAndResourcesArray(source: LhqModelType): LhqCategoriesAndResourcesType {
        let result: LhqCategoriesAndResourcesType = {};

        function recursiveCategories(parent: LhqCategoriesAndResourcesType, sourceCategories?: LhqModelCategoriesCollectionType) {
            if (sourceCategories) {
                const keys = Object.keys(sourceCategories);
                if (keys.length > 0) {
                    if (parent.categories === undefined) {
                        parent.categories = [];
                    }

                    keys.forEach(key => {
                        const sourceCategory = sourceCategories[key];
                        const newCategory: LhqModelCategoryWithNameType = {name: key};
                        recursiveCategories(newCategory, sourceCategory.categories);
                        recursiveResources(newCategory, sourceCategory.resources);

                        parent.categories!.push(newCategory);
                    });

                    parent.categories = arraySortBy(parent.categories, x => x.name);
                    parent.resources = arraySortBy(parent.resources, x => x.name);
                }
            }
        }

        function recursiveResources(parent: LhqCategoriesAndResourcesType, sourceResources?: LhqModelResourcesCollectionType) {
            if (sourceResources) {
                const keys = Object.keys(sourceResources);
                if (keys.length > 0) {
                    if (parent.resources === undefined) {
                        parent.resources = [];
                    }

                    keys.forEach(key => {
                        const sourceResource = sourceResources[key];
                        const newResource: LhqModelResourceWithNameType = Object.assign({}, sourceResource, {name: key});
                        parent.resources!.push(newResource);
                    });
                }
            }
        }

        recursiveCategories(result, source.categories);

        return result;
    }
}