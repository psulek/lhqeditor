import { TreeElement } from './treeElement';
import { ICategoryElement, ICategoryLikeTreeElement, IResourceElement, IRootModelElement, TreeElementType } from './api/types';
import { isNullOrEmpty, iterateObject, sortObjectByKey } from '../utils';
import { LhqModelCategoriesCollection, LhqModelCategory, LhqModelResource, LhqModelResourcesCollection } from './api/schemas';

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

        //this.setCategories(newCategories);
        this._categories = newCategories;
        this._hasCategories = this.categories.length > 0;
        
        //this.setResources(newResources);
        this._resources = newResources;
        this._hasResources = this.resources.length > 0;
    }

    // protected getIsFirst(): boolean {
    //     return this.parent?.categories.indexOf(this) === 0;
    // }

    // protected getIsLast(): boolean {
    //     if (this.parent?.categories?.length === 0) {
    //         return false;
    //     }
    //     return this.parent?.categories[this.parent.categories.length - 1] === this;
    // }

    // public setCategories(categories: ICategoryElement[]): void {
    //     this._categories = categories ?? [];
    // }

    // public setResources(resources: IResourceElement[]): void {
    //     this._resources = resources ?? [];
    // }


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
