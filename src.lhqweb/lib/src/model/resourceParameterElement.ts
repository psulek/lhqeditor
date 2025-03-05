import { LhqModelResourceParameter } from './schemas';
import { IResourceElement, IResourceParameterElement } from './types';

export class ResourceParameterElement implements IResourceParameterElement {
    private _name: string;
    private _description: string | undefined;
    private _order: number;
    private _parent: IResourceElement;

    constructor(name: string, source: LhqModelResourceParameter, parent: IResourceElement) {
        this._name = name;
        this._description = source.description;
        this._order = source.order ?? 0;
        this._parent = parent;
    }

    public get name(): string {
        return this._name;
    }

    public get parent(): Readonly<IResourceElement> {
        return this._parent;
    }
    
    public get description(): string | undefined {
        return this._description;
    }

    public get order(): number {
        return this._order;
    }
}

