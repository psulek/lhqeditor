import type {
    LhqModelOptions,
    LhqModelResourceTranslationState, LhqModelUid, LhqModelVersion, LhqModelMetadata,
    LhqCodeGenVersion,
    LhqModelLineEndings,
    LhqModelDataNode
} from './schemas';

export type TreeElementType = 'model' | 'category' | 'resource';

/**
 * Represents a tree element (root, category or resource) from `*.lhq` model file.
 */
export interface ITreeElement {
    /**
     * Gets the parent of the current tree element.
     * @remarks
     * For resource element, it will be category.
     * For category element, it will be either another category or root.
     * For root element, it will be undefined.
     */
    readonly parent: Readonly<ICategoryLikeTreeElement | undefined>;

    /**
     * Gets the root of the tree.
     */
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

export interface CodeGeneratorBasicSettings {
    OutputFolder: string;
    OutputProjectName?: string;
    EncodingWithBOM: boolean;
    LineEndings: LhqModelLineEndings;
    Enabled: boolean;
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

export type ICategoryElement = ICategoryLikeTreeElement;

export interface IResourceElement extends ITreeElement {
    readonly state: LhqModelResourceTranslationState;
    readonly parameters: Readonly<IResourceParameterElement[]>;
    readonly values: Readonly<IResourceValueElement[]>;
    readonly comment: string;
    readonly hasParameters: boolean;
    readonly hasValues: boolean;

    getValue(language: string, trim?: boolean): string;
    hasValue(language: string): boolean;
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