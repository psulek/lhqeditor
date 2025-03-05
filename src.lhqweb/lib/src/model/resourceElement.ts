import { isNullOrEmpty, iterateObject, sortObjectByKey, sortObjectByValue } from '../utils';
import { ResourceParameterElement } from './resourceParameterElement';
import { ResourceValueElement } from './resourceValueElement';
import { LhqModelResource, LhqModelResourceTranslationState } from './schemas';
import { TreeElement } from './treeElement';
import { ICategoryLikeTreeElement, IResourceElement, IResourceParameterElement, IResourceValueElement } from './types';

export class ResourceElement extends TreeElement implements IResourceElement {
    private _state: LhqModelResourceTranslationState;
    private _parameters: IResourceParameterElement[];
    private _values: IResourceValueElement[];

    constructor(name: string, source: LhqModelResource, parent: ICategoryLikeTreeElement) {
        super('resource', name, source.description, parent);

        this._state = source.state;

        this._parameters = [];
        if (!isNullOrEmpty(source.parameters)) {
            iterateObject(sortObjectByValue(source.parameters, x => x.order), (parameter, name) => {
                this._parameters.push(new ResourceParameterElement(name, parameter, this));
            });
        }

        this._values = [];
        if (!isNullOrEmpty(source.values)) {
            iterateObject(sortObjectByKey(source.values), (resValue, name) => {
                this._values.push(new ResourceValueElement(name, resValue, this));
            });
        }
    }

    protected getIsFirst(): boolean {
        return this.parent?.resources.indexOf(this) === 0;
    }
    
    protected getIsLast(): boolean {
        if (this.parent?.resources?.length === 0) {
            return false;
        }
        return this.parent?.resources[this.parent.resources.length - 1] === this;
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
