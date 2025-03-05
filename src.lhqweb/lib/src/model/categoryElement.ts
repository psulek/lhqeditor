import { CategoryLikeTreeElement } from './categoryLikeTreeElement';
import { LhqModelCategory } from './schemas';
import { ICategoryElement, ICategoryLikeTreeElement } from './types';

export class CategoryElement extends CategoryLikeTreeElement implements ICategoryElement {
    constructor(name: string, source: LhqModelCategory, parent: ICategoryLikeTreeElement | undefined) {
        super('category', name, source?.description, parent);
    }
}
