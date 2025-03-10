import {
    LhqModelDataNode, LhqModelOptions,
    LhqModelResourceTranslationState, LhqModelUid, LhqModelVersion, LhqModelMetadata,
    LhqCodeGenVersion
}
    from './schemas';

export type TreeElementType = 'model' | 'category' | 'resource';

export interface ITreeElement {
    readonly parent: Readonly<ICategoryLikeTreeElement | undefined>;
    readonly root: Readonly<IRootModelElement>;
    readonly name: string;
    readonly elementType: TreeElementType;
    readonly description: string | undefined;
    readonly paths: Readonly<ITreeElementPaths>;
    readonly isRoot: boolean;

    /**
     * temp data defined dynamically by template run, resets on each template run.
     */
    readonly data: Readonly<Record<string, unknown>>;
}

export interface ICategoryLikeTreeElement extends ITreeElement {
    readonly categories: Readonly<ICategoryElement[]>;
    readonly resources: Readonly<IResourceElement[]>;

    readonly hasCategories: boolean;
    readonly hasResources: boolean;
}

export interface ICodeGeneratorElement {
    readonly templateId: string;

    // TODO: Must add 'version' for code generator templates in v3 of LHQ model file format!!
    // for now this value will be artificially set to 1 (not stored in lhq model file)
    readonly version: LhqCodeGenVersion;
    readonly settings: LhqModelDataNode;
}

export interface IRootModelElement extends ICategoryLikeTreeElement {
    readonly uid: LhqModelUid;
    readonly version: LhqModelVersion;
    readonly description: string | undefined;
    readonly options: Readonly<LhqModelOptions>;
    readonly primaryLanguage: string;
    readonly languages: Readonly<string[]>;
    readonly hasLanguages: boolean;
    readonly metadatas: Readonly<LhqModelMetadata> | undefined;
    readonly codeGenerator: ICodeGeneratorElement | undefined;
}

export interface ICategoryElement extends ICategoryLikeTreeElement { }

export interface IResourceElement extends ITreeElement {
    readonly state: LhqModelResourceTranslationState;
    readonly parameters: Readonly<IResourceParameterElement[]>;
    readonly values: Readonly<IResourceValueElement[]>;
    readonly comment: string;
    readonly hasParameters: boolean;
    readonly hasValues: boolean;

    getValue(language: string): string;
}

export interface IResourceParameterElement {
    readonly name: string;
    readonly description: string | undefined;
    readonly order: number;
    readonly parent: Readonly<IResourceElement>;
}

export interface IResourceValueElement {
    readonly languageName: string;
    readonly value: string | undefined;
    readonly locked: boolean | undefined;
    readonly auto: boolean | undefined;
    readonly parent: Readonly<IResourceElement>;
}

export interface ITreeElementPaths {
    getParentPath(separator: string, includeRoot?: boolean): string;
}

export type ModelVersionsType = {
    model: LhqModelVersion;
    codeGenerator: LhqCodeGenVersion;
}