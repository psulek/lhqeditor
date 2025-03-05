import { LhqModelDataNode, LhqModelOptions, LhqModelResourceTranslationState, LhqModelUid, LhqModelVersion, LqhModelMetadata } from './schemas';

export type TreeElementType = 'model' | 'category' | 'resource';

export interface ITreeElement {
    readonly parent: Readonly<ICategoryLikeTreeElement | undefined>;
    readonly name: string;
    readonly elementType: TreeElementType;
    readonly description: string | undefined;
    readonly paths: Readonly<ITreeElementPaths>;
    readonly isFirst: boolean;
    readonly isLast: boolean;
    readonly isRoot: boolean;
}

export interface ICategoryLikeTreeElement extends ITreeElement {
    readonly categories: Readonly<ICategoryElement[]>;
    readonly resources: Readonly<IResourceElement[]>;
}

export interface ICodeGeneratorElement {
    readonly templateId: string;

    // TODO: Must add 'version' for code generator templates in v3 of LHQ model file format!!
    // readonly version: string; 
    readonly settings: LhqModelDataNode;
}

export interface IRootModelElement extends ICategoryLikeTreeElement {
    readonly uid: LhqModelUid;
    readonly version: LhqModelVersion;
    readonly description: string | undefined;
    readonly options: Readonly<LhqModelOptions>;
    readonly primaryLanguage: string;
    readonly languages: Readonly<string[]>;
    readonly metadatas: Readonly<LqhModelMetadata> | undefined;
    readonly codeGenerator: ICodeGeneratorElement | undefined;
}

export interface ICategoryElement extends ICategoryLikeTreeElement {}

export interface IResourceElement extends ITreeElement {
    readonly state: LhqModelResourceTranslationState;
    readonly parameters: Readonly<IResourceParameterElement[]>;
    readonly values: Readonly<IResourceValueElement[]>;
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
    getParentPath(separator: string): string;
}