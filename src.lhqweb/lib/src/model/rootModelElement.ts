import { CategoryLikeTreeElement } from './categoryLikeTreeElement';
import { LhqCodeGenVersion, LhqModel, LhqModelCategory, LhqModelResource, LhqModelUid, LhqModelVersion } from '../api/schemas';
import { ICategoryLikeTreeElement, ICodeGeneratorElement, IResourceElement, IRootModelElement } from '../api/modelTypes';
import { isNullOrEmpty } from '../utils';
import { ModelVersions } from './modelConst';
import { CategoryElement } from './categoryElement';
import { ResourceElement } from './resourceElement';

const CodeGenUID = 'b40c8a1d-23b7-4f78-991b-c24898596dd2';

export class RootModelElement extends CategoryLikeTreeElement implements IRootModelElement {
    private _uid: LhqModelUid;
    private _version: LhqModelVersion;
    private _options: Readonly<{ categories: boolean; resources: 'All' | 'Categories'; }>;
    private _primaryLanguage: string;
    private _languages: readonly string[];
    private _metadatas: Readonly<{ childs?: any[] }> | undefined;
    private _codeGenerator: ICodeGeneratorElement | undefined;
    private _hasLanguages: boolean;

    constructor(model: LhqModel) {
        // @ts-ignore
        super(undefined, 'model', model.model.name ?? '', model.model.description ?? '', undefined);
        this._uid = model.model.uid;
        this._version = model.model.version;
        this._options = { categories: model.model.options.categories, resources: model.model.options.resources };
        this._primaryLanguage = model.model.primaryLanguage;
        this._languages = Object.freeze([...model.languages]);
        this._hasLanguages = this._languages.length > 0;
        this._metadatas = model.metadatas ? Object.freeze({ ...model.metadatas }) : undefined;
        this._codeGenerator = this.getCodeGenerator(model);
        this.populate(model.categories, model.resources);
    }

    protected createCategory(root: IRootModelElement, name: string, source: LhqModelCategory,
        parent: ICategoryLikeTreeElement | undefined): CategoryLikeTreeElement {
        return new CategoryElement(root, name, source, parent);
    }

    protected createResource(root: IRootModelElement, name: string, source: LhqModelResource, parent: ICategoryLikeTreeElement): IResourceElement {
        return new ResourceElement(root, name, source, parent);
    }

    private getCodeGenerator(model: LhqModel): ICodeGeneratorElement | undefined {
        let templateId = '';
        let codeGenVersion: LhqCodeGenVersion = 1;
        let node = model.metadatas?.childs?.find(x => x.name === 'metadata' && x.attrs?.['descriptorUID'] === CodeGenUID);
        if (node) {
            node = node.childs?.find(x => x.name === 'content' && x.attrs?.['templateId'] !== undefined);
            if (node) {
                templateId = node.attrs!['templateId'];
                const version = node.attrs!['version'];
                if (!isNullOrEmpty(version)) {
                    const versionInt = parseInt(version);
                    if (versionInt > 0 && versionInt <= ModelVersions.codeGenerator) {
                        codeGenVersion = versionInt as LhqCodeGenVersion;
                    }
                }
                node = node.childs?.find(x => x.name === 'Settings' && (x.childs?.length ?? 0) > 0);
            }
        }

        if (!isNullOrEmpty(templateId) && !isNullOrEmpty(node)) {
            

            return { templateId, settings: node, version: codeGenVersion };
        }
    }

    get uid(): LhqModelUid {
        return this._uid;
    }

    get version(): LhqModelVersion {
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

    get hasLanguages(): boolean {
        return this._hasLanguages;
    }

    get metadatas(): Readonly<{ childs?: any[] }> | undefined {
        return this._metadatas;
    }

    get codeGenerator(): ICodeGeneratorElement | undefined {
        return this._codeGenerator;
    }
}