import { TreeElement } from './treeElement';
import { ICategoryElement, ICategoryLikeTreeElement, IResourceElement, TreeElementType } from './types';
import { isNullOrEmpty, iterateObject, sortObjectByKey, sortObjectByValue } from '../utils';
import { CategoryElement } from './categoryElement';
import { ResourceElement } from './resourceElement';
import { LhqModelCategoriesCollection, LhqModelResourcesCollection } from './schemas';

export abstract class CategoryLikeTreeElement extends TreeElement implements ICategoryLikeTreeElement {
    private _categories: ICategoryElement[];
    private _resources: IResourceElement[];

    constructor(elementType: TreeElementType, name: string, description: string | undefined, parent: ICategoryLikeTreeElement | undefined) {
        super(elementType, name, description, parent);
        this._categories = [];
        this._resources = [];
    }

    protected populate(sourceCategories: LhqModelCategoriesCollection | undefined,
        sourceResources: LhqModelResourcesCollection | undefined): void {

        const newCategories: ICategoryElement[] = [];
        const newResources: ResourceElement[] = [];

        if (!isNullOrEmpty(sourceCategories)) {
            iterateObject(sortObjectByKey(sourceCategories), (category, name) => {
                const newCategory = new CategoryElement(name, category, this);
                newCategories.push(newCategory);
                newCategory.populate(category.categories, category.resources);
            });
        }

        if (!isNullOrEmpty(sourceResources)) {
            iterateObject(sortObjectByKey(sourceResources), (resource, name) => {
                const newResource = new ResourceElement(name, resource, this);
                newResources.push(newResource);
            });
        }

        this.setCategories(newCategories);
        this.setResources(newResources);
    }

    protected getIsFirst(): boolean {
        return this.parent?.categories.indexOf(this) === 0;
    }
    
    protected getIsLast(): boolean {
        if (this.parent?.categories?.length === 0) {
            return false;
        }
        return this.parent?.categories[this.parent.categories.length - 1] === this;
    }

    public setCategories(categories: ICategoryElement[]): void {
        this._categories = categories ?? [];
    }

    public setResources(resources: IResourceElement[]): void {
        this._resources = resources ?? [];
    }

    public get categories(): Readonly<ICategoryElement[]> {
        return this._categories;
    }

    public get resources(): Readonly<IResourceElement[]> {
        return this._resources;
    }
    
    public get hasCategories(): boolean {
        return this.categories.length > 0;
    }

    public get hasResources(): boolean {
        return this.resources.length > 0;
    }
}
