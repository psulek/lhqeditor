import type { ICategoryElement, ICategoryLikeTreeElement, IResourceElement, IRootModelElement, TreeElementType } from '../api/modelTypes.mjs';
import type { LhqModelCategoriesCollection, LhqModelCategory, LhqModelResource, LhqModelResourcesCollection } from '../api/schemas.mjs';
import { isNullOrEmpty, iterateObject, sortObjectByKey } from '../utils.mjs';
import { TreeElement } from './treeElement.mjs';

export abstract class CategoryLikeTreeElement extends TreeElement implements ICategoryLikeTreeElement {
    private _categories: ICategoryElement[];
    private _resources: IResourceElement[];
    private _hasCategories: boolean;
    private _hasResources: boolean;

    constructor(root: IRootModelElement, elementType: TreeElementType, name: string,
        description: string | undefined, parent: ICategoryLikeTreeElement | undefined) {
        super(root, elementType, name, description, parent);
        this._categories = [];
        this._resources = [];
        this._hasCategories = false;
        this._hasResources = false;
    }

    protected abstract createCategory(root: IRootModelElement, name: string, source: LhqModelCategory,
        parent: ICategoryLikeTreeElement | undefined): CategoryLikeTreeElement;

    protected abstract createResource(root: IRootModelElement, name: string, source: LhqModelResource, 
        parent: ICategoryLikeTreeElement): IResourceElement;

    protected populate(sourceCategories: LhqModelCategoriesCollection | undefined,
        sourceResources: LhqModelResourcesCollection | undefined): void {

        const newCategories: ICategoryElement[] = [];
        const newResources: IResourceElement[] = [];

        if (!isNullOrEmpty(sourceCategories)) {
            iterateObject(sortObjectByKey(sourceCategories), (category, name) => {
                const newCategory = this.createCategory(this.root, name, category, this);
                newCategories.push(newCategory);
                newCategory.populate(category.categories, category.resources);
            });
        }

        if (!isNullOrEmpty(sourceResources)) {
            iterateObject(sortObjectByKey(sourceResources), (resource, name) => {
                const newResource = this.createResource(this.root, name, resource, this);
                newResources.push(newResource);
            });
        }

        this._categories = newCategories;
        this._hasCategories = this.categories.length > 0;
        
        this._resources = newResources;
        this._hasResources = this.resources.length > 0;
    }

    public get categories(): Readonly<ICategoryElement[]> {
        return this._categories;
    }

    public get resources(): Readonly<IResourceElement[]> {
        return this._resources;
    }

    public get hasCategories(): boolean {
        return this._hasCategories;
    }

    public get hasResources(): boolean {
        return this._hasResources;
    }
}
