import { isNullOrEmpty } from '../utils';
import { TreeElementPaths } from './treeElementPaths';
import { ICategoryLikeTreeElement, ITreeElement, ITreeElementPaths, TreeElementType } from './types';

export abstract class TreeElement implements ITreeElement {
    private _parent: ICategoryLikeTreeElement | undefined;
    private _name: string;
    private _elementType: TreeElementType;
    private _description: string | undefined;
    private _paths: ITreeElementPaths;
    private _isFirst: boolean;
    private _isLast: boolean;
    private _isRoot: boolean;

    constructor(elementType: TreeElementType, name: string, description: string | undefined,
         parent: ICategoryLikeTreeElement | undefined) {

        this._name = name ?? '';
        this._elementType = elementType;
        this._description = description ?? '';
        this._parent = parent;
        this._paths = new TreeElementPaths(this);
        this._isRoot = isNullOrEmpty(this.parent);

        this._isFirst = this._isRoot ? true : this.getIsFirst();
        this._isLast = this._isRoot ? true : this.getIsLast();
    }

    protected abstract getIsFirst(): boolean;
    protected abstract getIsLast(): boolean;

    public get isRoot(): boolean {
        return this._isRoot;
    }

    public get parent(): Readonly<ICategoryLikeTreeElement | undefined> {
        return this._parent;
    }

    public get name(): string {
        return this._name;
    }

    public get elementType(): TreeElementType {
        return this._elementType;
    }

    public get description(): string | undefined {
        return this._description;
    }

    public get paths(): Readonly<ITreeElementPaths> {
        return this._paths;
    }

    public get isFirst(): boolean {
        return this._isFirst;
    }

    public get isLast(): boolean {
        return this._isLast;
    }
}