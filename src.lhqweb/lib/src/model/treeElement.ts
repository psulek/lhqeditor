import { isNullOrEmpty } from '../utils';
import { ITreeElement, ICategoryLikeTreeElement, IRootModelElement, TreeElementType, ITreeElementPaths } from './api/types';
import { TreeElementPaths } from './treeElementPaths';

export abstract class TreeElement implements ITreeElement {
    private _parent: ICategoryLikeTreeElement | undefined;
    private _root: IRootModelElement;
    private _name: string;
    private _elementType: TreeElementType;
    private _description: string | undefined;
    private _paths: ITreeElementPaths;
    private _isRoot: boolean;
    private _data: Record<string, unknown>;

    constructor(root: IRootModelElement, elementType: TreeElementType, name: string, description: string | undefined,
        parent: ICategoryLikeTreeElement | undefined) {

        this._name = name ?? '';
        this._elementType = elementType;
        this._description = description ?? '';
        this._root = isNullOrEmpty(root) && isNullOrEmpty(parent) ? this as unknown as IRootModelElement : root;
        this._parent = parent;
        this._paths = new TreeElementPaths(this);
        this._isRoot = isNullOrEmpty(this.parent);
        this._data = {};
    }

    public addToTempData = (key: string, value: unknown): void => {
        this._data[key] = value;
    }

    public clearTempData = (): void => {
        this._data = {};
    }

    public get isRoot(): boolean {
        return this._isRoot;
    }

    public get root(): IRootModelElement {
        return this._root;
    }

    public get parent(): Readonly<ICategoryLikeTreeElement | undefined> {
        return this._parent;
    }

    public get data(): Readonly<Record<string, unknown>> {
        return this._data;
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
}