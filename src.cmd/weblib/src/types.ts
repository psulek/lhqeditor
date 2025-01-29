export type LhqModelType = Record<string, unknown> & {
    model: {
        uid: string,
        version: 1,
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

export type LhqCategoriesAndResourcesType = {
    categories?: Array<LhqModelCategoryWithNameType>;
    resources?: Array<LhqModelResourceWithNameType>;
}

export type LhqModelCategoryWithNameType = LhqCategoriesAndResourcesType & { name: string; }

export type LhqModelCategoriesCollectionType = Record<string, LhqModelCategoryType>; 
export type LhqModelResourcesCollectionType = Record<string, LhqModelResourceType>; 

export type LhqModelCategoryType = {
    categories?: LhqModelCategoriesCollectionType;
    resources?: LhqModelResourcesCollectionType;
}

export type LhqModelResourceWithNameType = LhqModelResourceType & {name: string};

export type LhqModelResourceType = {
    state: string;
    description: string;
    parameters: Record<string, {
        description: string;
        order: number;
    }>;
    values: Record<string, {
        value: string;
    }>;
}

export type TemplateRootModel = {
    model: LhqModelType;
    settings: unknown;
    host: Record<string, unknown>;
    // multiple stages for 1 handlebars template
    //stage?: string;
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