import { CategoryLikeTreeElement } from './categoryLikeTreeElement.mjs';
import type { LhqModelCategory, LhqModelResource } from '../api/schemas.mjs';
import type { ICategoryElement, ICategoryLikeTreeElement, IResourceElement, IRootModelElement } from '../api/modelTypes.mjs';
import { ResourceElement } from './resourceElement.mjs';

export class CategoryElement extends CategoryLikeTreeElement implements ICategoryElement {
    constructor(root: IRootModelElement, name: string, source: LhqModelCategory,
        parent: ICategoryLikeTreeElement | undefined) {
        super(root, 'category', name, source?.description, parent);
    }

    protected createCategory(root: IRootModelElement, name: string, source: LhqModelCategory,
        parent: ICategoryLikeTreeElement | undefined): CategoryLikeTreeElement {
        return new CategoryElement(root, name, source, parent);
    }

    protected createResource(root: IRootModelElement, name: string, source: LhqModelResource, parent: ICategoryLikeTreeElement): IResourceElement {
        return new ResourceElement(root, name, source, parent);
    }
}
