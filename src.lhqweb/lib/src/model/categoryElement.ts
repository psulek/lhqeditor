import { CategoryLikeTreeElement } from './categoryLikeTreeElement';
import { LhqModelCategory } from './api/schemas';
import { ICategoryElement, ICategoryLikeTreeElement } from './api/types';

export class CategoryElement extends CategoryLikeTreeElement implements ICategoryElement {
    constructor(name: string, source: LhqModelCategory, parent: ICategoryLikeTreeElement | undefined) {
        super('category', name, source?.description, parent);
    }
}
