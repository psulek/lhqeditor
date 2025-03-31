import { isNullOrEmpty, iterateObject, sortObjectByKey, sortObjectByValue, trimComment } from '../utils';
import { ResourceParameterElement } from './resourceParameterElement';
import { ResourceValueElement } from './resourceValueElement';
import type { LhqModelResource, LhqModelResourceTranslationState } from '../api/schemas';
import type { IResourceElement, IResourceParameterElement, IResourceValueElement, IRootModelElement, ICategoryLikeTreeElement } from '../api/modelTypes';
import { TreeElement } from './treeElement';

export class ResourceElement extends TreeElement implements IResourceElement {
    private _state: LhqModelResourceTranslationState;
    private _parameters: IResourceParameterElement[];
    private _values: IResourceValueElement[];
    private _comment: string;
    private _hasParameters: boolean;
    private _hasValues: boolean;

    constructor(root: IRootModelElement, name: string, source: LhqModelResource, parent: ICategoryLikeTreeElement) {
        super(root, 'resource', name, source.description, parent);

        this._state = source.state;

        this._parameters = [];
        if (!isNullOrEmpty(source.parameters)) {
            iterateObject(sortObjectByValue(source.parameters, x => x.order), (parameter, name) => {
                this._parameters.push(new ResourceParameterElement(name, parameter, this));
            });
        }
        this._hasParameters = this.parameters.length > 0;

        this._values = [];
        if (!isNullOrEmpty(source.values)) {
            iterateObject(sortObjectByKey(source.values), (resValue, name) => {
                this._values.push(new ResourceValueElement(name, resValue, this));
            });
        }
        this._hasValues = this.values.length > 0;
        this._comment = this.getComment();
    }

    public get hasParameters(): boolean {
        return this._hasParameters;
    }
    
    public get hasValues(): boolean {
        return this._hasValues;
    }

    public get comment(): string {
        return this._comment;
    }

    private getComment = (): string => {
        const root = this.root;
        const primaryLanguage = root.primaryLanguage ?? '';
        if (!isNullOrEmpty(primaryLanguage) && this.values) {
            const value = this.values.find(x => x.languageName === primaryLanguage);
            const resourceValue = value?.value;
            const propertyComment = (isNullOrEmpty(resourceValue) ? this.description : resourceValue) ?? '';
            return trimComment(propertyComment);
        }

        return '';
    }
    
    public getValue = (language: string, trim?: boolean): string => {
        let result = '';
        if (!isNullOrEmpty(language) && this.values) {
            const value = this.values.find(x => x.languageName === language);
            result = value?.value ?? '';
        }

        return trim === true ? result.trim() : result;
    }

    public hasValue = (language: string): boolean => {
        if (!isNullOrEmpty(language) && this.values) {
            const value = this.values.find(x => x.languageName === language);
            return !isNullOrEmpty(value);
        }

        return false;
    }

    public get state(): LhqModelResourceTranslationState {
        return this._state;
    }

    public get parameters(): Readonly<IResourceParameterElement[]> {
        return this._parameters;
    }

    public get values(): Readonly<IResourceValueElement[]> {
        return this._values;
    }
}
