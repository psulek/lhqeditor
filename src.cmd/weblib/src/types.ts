export type LhqModelType = Record<string, unknown> & {
    model: {
        uid: string,
        version: number,
        options: {
            categories: boolean,
            resources: 'All' | 'Categories'
        },
        name: string,
        primaryLanguage: string
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
}

export type LhqModelResourceParameterType = {
    description: string;
    order: number;
}

export type LhqModelResourceType = {
    state: string;
    description: string;
    parameters: Record<string, LhqModelResourceParameterType>;
    values: Record<string, {
        value: string;
    }>;
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
    Enabled: boolean;
}

export interface CSharpGeneratorSettings extends GeneratorSettings {
    UseExpressionBodySyntax: boolean;
    RootNamespace: string;
    MissingTranslationFallbackToPrimary: boolean;
}

export interface CSharpWinFormsGeneratorSettings extends CSharpGeneratorSettings {
    GenerateParamsMethods: boolean;
    ParamsMethodsSuffix: string;
}

export interface JsonGeneratorSettings extends GeneratorSettings {
    CultureCodeInFileNameForPrimaryLanguage: boolean;
    WriteEmptyValues: boolean;
    MetadataFileNameSuffix: string;
}

export interface TypescriptGeneratorSettings extends GeneratorSettings {
    AmbientNamespaceName: string;
    InterfacePrefix: string;
}

export interface ResXGeneratorSettings extends GeneratorSettings {
    CultureCodeInFileNameForPrimaryLanguage: boolean;
}

// export type ModelSettings = {
//     templateId: string;
//     CSharp: {};
//     ResX: {
//         Enabled: boolean;
//         OutputFolder: string;
//         OutputProjectName: string;
//         CultureCodeInFileNameForPrimaryLanguage: boolean;
//     }
// }

export type ModelDataNode = {
    name: string;
    attrs: Record<string, string>;
    childs: ModelDataNode[];
}