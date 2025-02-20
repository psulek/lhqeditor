// noinspection JSUnusedGlobalSymbols

import {
    LhqModelCategoryOrResourceType,
    LhqModelCategoryType,
    LhqModelType,
    ModelDataNode,
    TemplateRootModel
} from "./types";
import {clearHelpersContext, debugHelpersTimeTaken, registerHelpers} from "./helpers";
import {TypescriptJson01Template} from "./templates/typescriptJson";
import {CodeGeneratorTemplate, CodeGeneratorTemplateConstructor} from "./templates/codeGeneratorTemplate";
import {NetCoreResxCsharp01Template} from "./templates/netCoreResxCsharp";
import {NetFwResxCsharp01Template} from "./templates/netFwResxCsharp";
import {WinFormsResxCsharp01Template} from "./templates/winFormsResxCsharp";
import {WpfResxCsharp01Template} from "./templates/wpfResxCsharp";
import {hasItems, iterateObject, sortObjectByKey, sortObjectByValue} from "./utils";
import {HostEnv} from "./hostEnv";

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

        // @ts-ignore
        String.prototype.isTrue = function () {
            return this.toLowerCase() === "true";
        };

        registerHelpers();
    }

    public static runTemplate(lhqModelJson: string, hostData: string): void {
        let lhqModel = JSON.parse(lhqModelJson) as LhqModelType;
        if (lhqModel) {
            //const startTime = HostEnv.stopwatchStart();
            
            lhqModel = TemplateManager.sortByNameModel(lhqModel);
            
            // const elapsedTime = HostEnv.stopwatchEnd(startTime);
            // HostEnv.debugLog(`[sortByNameModel] takes ${elapsedTime}`);

            const {template, templateId, settingsNode} = TemplateManager.loadTemplate(lhqModel as LhqModelType);
            let settings = template.loadSettings(settingsNode);
            let host = {};
            if (hostData) {
                host = JSON.parse(hostData) as Record<string, unknown>;
            }

            const rootModel: TemplateRootModel = {
                model: lhqModel,
                settings: settings,
                host: host,
                extra: {}
            };

            template.generate(rootModel);

            debugHelpersTimeTaken();

            clearHelpersContext();
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
            const ctor = TemplateManager.generators[templateId];
            template = (ctor && new ctor(TemplateManager.handlebarFiles)) || undefined;

            return {template, templateId, settingsNode: node};
        }

        throw new Error(`Template '${templateId}' not found !`);
    }

    private static sortByNameModel(lhqModel: LhqModelType): LhqModelType {
        function getFullParentPath(sep: string, element: LhqModelCategoryOrResourceType): string {
            let pathArray: string[] = [];
            let currentElement: LhqModelCategoryType | undefined = element.getParent!();
            pathArray.unshift(element.getName!());

            while (currentElement) {
                if (currentElement?.isRoot!()) {
                    break;
                }
                pathArray.unshift(currentElement.getName!());
                currentElement = currentElement.getParent!();
            }
            
            return pathArray.join(sep);
        }

        function recursiveCategories(parentCategory: LhqModelCategoryType) {
            if (parentCategory.categories) {
                parentCategory.categories = sortObjectByKey(parentCategory.categories);
                iterateObject(parentCategory.categories, (category, name, __, isLastCategory) => {
                    category.getName = () => name;
                    category.isRoot = () => false;
                    category.isLast = () => isLastCategory;
                    category.getParent = () => parentCategory;
                    category.hasCategories = () => hasItems(parentCategory.categories);
                    category.hasResources = () => hasItems(parentCategory.resources);
                    category.getFullParentPath = (sep: string) => getFullParentPath(sep, category);

                    recursiveCategories(category);
                });
            }
            if (parentCategory.resources) {
                parentCategory.resources = sortObjectByKey(parentCategory.resources);

                iterateObject(parentCategory.resources, (resource, name, __, isLastResource) => {
                    resource.getName = () => name;
                    resource.isLast = () => isLastResource;
                    resource.getParent = () => parentCategory;
                    resource.hasParameters = () => hasItems(resource.parameters);
                    resource.getFullParentPath = (sep: string) => getFullParentPath(sep, resource);

                    if (resource.parameters) {
                        resource.parameters = sortObjectByValue(resource.parameters, x => x.order);

                        iterateObject(resource.parameters, (parameter, _, __, isLastParam) => {
                            parameter.isLast = () => isLastParam;
                            parameter.getParent = () => resource;
                        })
                    }
                });
            }
        }

        lhqModel.getName = () => lhqModel.model.name;
        lhqModel.isRoot = () => true;
        lhqModel.isLast = () => true;
        lhqModel.getParent = () => undefined;
        lhqModel.hasCategories = () => hasItems(lhqModel.categories);
        lhqModel.hasResources = () => hasItems(lhqModel.resources);
        lhqModel.getFullParentPath = () => '';
        recursiveCategories(lhqModel);
        return lhqModel;
    }
}