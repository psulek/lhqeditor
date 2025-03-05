import { CategoryLikeTreeElement } from './categoryLikeTreeElement';
import { LhqModel, LhqModelUid, LhqModelVersion } from './schemas';
import { ICodeGeneratorElement, IRootModelElement } from './types';

export class RootModelElement extends CategoryLikeTreeElement implements IRootModelElement {
    private _uid: LhqModelUid;
    private _version: LhqModelVersion;
    private _options: Readonly<{ categories: boolean; resources: 'All' | 'Categories'; }>;
    private _primaryLanguage: string;
    private _languages: readonly string[];
    private _metadatas: Readonly<{ childs?: any[] }> | undefined;
    private _codeGenerator: ICodeGeneratorElement | undefined;

    constructor(model: LhqModel) {
        super('model', model.model.name ?? '', model.model.description ?? '', undefined);
        this._uid = model.model.uid;
        this._version = model.model.version;
        this._options = { categories: model.model.options.categories, resources: model.model.options.resources };
        this._primaryLanguage = model.model.primaryLanguage;
        this._languages = Object.freeze([...model.languages]);
        this._metadatas = model.metadatas ? Object.freeze({ ...model.metadatas }) : undefined;
        this._codeGenerator = model.codeGenerator;
        this.populate(model.categories, model.resources);
    }

    get uid(): '6ce4d54c5dbd415c93019d315e278638' {
        return this._uid;
    }

    get version(): LhqModelUid {
        return this._version;
    }

    get options(): Readonly<{ categories: boolean; resources: 'All' | 'Categories'; }> {
        return this._options;
    }

    get primaryLanguage(): string {
        return this._primaryLanguage;
    }

    get languages(): readonly string[] {
        return this._languages;
    }

    get metadatas(): Readonly<{ childs?: any[] }> | undefined {
        return this._metadatas;
    }

    get codeGenerator(): ICodeGeneratorElement | undefined {
        return this._codeGenerator;
    }
}