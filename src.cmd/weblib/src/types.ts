export type BoolString = 'true' | 'false';

declare global {
    interface String {
        isTrue(): boolean;
    }
}

export type LhqModelType = Record<string, unknown> & {
    model: {
        uid: string;
        version: number;
        options: {
            categories: BoolString;
            resources: 'All' | 'Categories'
        };
        name: string;
        primaryLanguage: string;
    },
    languages: string[],
    metadatas: {
        childs: ModelDataNode[];
    }
} & LhqModelCategoryType;

export type LhqModelCategoriesCollectionType = Record<string, LhqModelCategoryType>; 

export type LhqModelCategoryType = {
    categories?: LhqModelCategoriesCollectionType;
    resources?: Record<string, LhqModelResourceType>;
    isRoot?: () => boolean;
    isLast?: () => boolean;
    getParent?: () => LhqModelCategoryType | undefined  ;
    hasCategories?: () => boolean;
    hasResources?: () => boolean;
}

export type LhqModelResourceParameterType = {
    description: string;
    order: number;
    isLast?: () => boolean;
    getParent?: () => LhqModelResourceType;
}

export type LhqModelResourceType = {
    state: string;
    description: string;
    parameters: Record<string, LhqModelResourceParameterType>;
    values: Record<string, {
        value: string;
    }>;
    isLast?: () => boolean;
    getParent?: () => LhqModelCategoryType;
    hasParameters?: () => boolean;
}

export type TemplateRootModel = {
    model: LhqModelType;
    settings: unknown;
    
    /*
     * data from host environment
     */
    host: Record<string, unknown>;
    
    /*
     * extra data defined dynamically by template
     */
    extra: Record<string, unknown>;
};

export interface OutputSettings {
    OutputFolder: string;
    OutputProjectName: string;
}

export interface GeneratorSettings extends OutputSettings {
    Enabled: BoolString;
}

export interface CSharpGeneratorSettings extends GeneratorSettings {
    UseExpressionBodySyntax: BoolString;
    RootNamespace: string;
    MissingTranslationFallbackToPrimary: BoolString;
}

export interface CSharpWinFormsGeneratorSettings extends CSharpGeneratorSettings {
    GenerateParamsMethods: BoolString;
    ParamsMethodsSuffix: string;
}

export interface JsonGeneratorSettings extends GeneratorSettings {
    CultureCodeInFileNameForPrimaryLanguage: BoolString;
    WriteEmptyValues: BoolString;
    MetadataFileNameSuffix: string;
}

export interface TypescriptGeneratorSettings extends GeneratorSettings {
    AmbientNamespaceName: string;
    InterfacePrefix: string;
}

export interface ResXGeneratorSettings extends GeneratorSettings {
    CultureCodeInFileNameForPrimaryLanguage: BoolString;
    CompatibleTextEncoding: BoolString;
}

export type ModelDataNode = {
    name: string;
    attrs: Record<string, string>;
    childs: ModelDataNode[];
}