import { z } from 'zod';

declare const lhqModelOptionsResourcesSchema: z.ZodUnion<[z.ZodLiteral<"All">, z.ZodLiteral<"Categories">]>;
declare const lhqModelResourceParameterSchema: z.ZodObject<{
    description: z.ZodOptional<z.ZodString>;
    order: z.ZodNumber;
}, "strip", z.ZodTypeAny, {
    order: number;
    description?: string | undefined;
}, {
    order: number;
    description?: string | undefined;
}>;
declare const lhqModelResourceTranslationStateSchema: z.ZodUnion<[z.ZodLiteral<"New">, z.ZodLiteral<"Edited">, z.ZodLiteral<"NeedsReview">, z.ZodLiteral<"Final">]>;
declare const lhqModelResourceValueScheme: z.ZodObject<{
    value: z.ZodOptional<z.ZodString>;
    locked: z.ZodOptional<z.ZodBoolean>;
    auto: z.ZodOptional<z.ZodBoolean>;
}, "strip", z.ZodTypeAny, {
    value?: string | undefined;
    locked?: boolean | undefined;
    auto?: boolean | undefined;
}, {
    value?: string | undefined;
    locked?: boolean | undefined;
    auto?: boolean | undefined;
}>;
declare const lhqModelResourceSchemaBase: z.ZodObject<{
    state: z.ZodUnion<[z.ZodLiteral<"New">, z.ZodLiteral<"Edited">, z.ZodLiteral<"NeedsReview">, z.ZodLiteral<"Final">]>;
    description: z.ZodOptional<z.ZodString>;
    parameters: z.ZodOptional<z.ZodRecord<z.ZodString, z.ZodObject<{
        description: z.ZodOptional<z.ZodString>;
        order: z.ZodNumber;
    }, "strip", z.ZodTypeAny, {
        order: number;
        description?: string | undefined;
    }, {
        order: number;
        description?: string | undefined;
    }>>>;
    values: z.ZodRecord<z.ZodString, z.ZodObject<{
        value: z.ZodOptional<z.ZodString>;
        locked: z.ZodOptional<z.ZodBoolean>;
        auto: z.ZodOptional<z.ZodBoolean>;
    }, "strip", z.ZodTypeAny, {
        value?: string | undefined;
        locked?: boolean | undefined;
        auto?: boolean | undefined;
    }, {
        value?: string | undefined;
        locked?: boolean | undefined;
        auto?: boolean | undefined;
    }>>;
}, "strip", z.ZodTypeAny, {
    values: Record<string, {
        value?: string | undefined;
        locked?: boolean | undefined;
        auto?: boolean | undefined;
    }>;
    state: "New" | "Edited" | "NeedsReview" | "Final";
    description?: string | undefined;
    parameters?: Record<string, {
        order: number;
        description?: string | undefined;
    }> | undefined;
}, {
    values: Record<string, {
        value?: string | undefined;
        locked?: boolean | undefined;
        auto?: boolean | undefined;
    }>;
    state: "New" | "Edited" | "NeedsReview" | "Final";
    description?: string | undefined;
    parameters?: Record<string, {
        order: number;
        description?: string | undefined;
    }> | undefined;
}>;
type Resource = z.infer<typeof lhqModelResourceSchemaBase> & {};
declare const lhqModelResourceSchema: z.ZodType<Resource>;
declare const baseDataNodeSchema: z.ZodObject<{
    name: z.ZodString;
    attrs: z.ZodOptional<z.ZodRecord<z.ZodString, z.ZodString>>;
}, "strip", z.ZodTypeAny, {
    name: string;
    attrs?: Record<string, string> | undefined;
}, {
    name: string;
    attrs?: Record<string, string> | undefined;
}>;
type BaseDataNode = z.infer<typeof baseDataNodeSchema> & {
    childs?: BaseDataNode[];
};
declare const lqhModelDataNodeSchema: z.ZodType<BaseDataNode>;
declare const baseCategorySchema: z.ZodObject<{
    description: z.ZodOptional<z.ZodString>;
    resources: z.ZodOptional<z.ZodLazy<z.ZodRecord<z.ZodString, z.ZodType<{
        values: Record<string, {
            value?: string | undefined;
            locked?: boolean | undefined;
            auto?: boolean | undefined;
        }>;
        state: "New" | "Edited" | "NeedsReview" | "Final";
        description?: string | undefined;
        parameters?: Record<string, {
            order: number;
            description?: string | undefined;
        }> | undefined;
    }, z.ZodTypeDef, {
        values: Record<string, {
            value?: string | undefined;
            locked?: boolean | undefined;
            auto?: boolean | undefined;
        }>;
        state: "New" | "Edited" | "NeedsReview" | "Final";
        description?: string | undefined;
        parameters?: Record<string, {
            order: number;
            description?: string | undefined;
        }> | undefined;
    }>>>>;
}, "strip", z.ZodTypeAny, {
    description?: string | undefined;
    resources?: Record<string, {
        values: Record<string, {
            value?: string | undefined;
            locked?: boolean | undefined;
            auto?: boolean | undefined;
        }>;
        state: "New" | "Edited" | "NeedsReview" | "Final";
        description?: string | undefined;
        parameters?: Record<string, {
            order: number;
            description?: string | undefined;
        }> | undefined;
    }> | undefined;
}, {
    description?: string | undefined;
    resources?: Record<string, {
        values: Record<string, {
            value?: string | undefined;
            locked?: boolean | undefined;
            auto?: boolean | undefined;
        }>;
        state: "New" | "Edited" | "NeedsReview" | "Final";
        description?: string | undefined;
        parameters?: Record<string, {
            order: number;
            description?: string | undefined;
        }> | undefined;
    }> | undefined;
}>;
type Category = z.infer<typeof baseCategorySchema> & {
    categories?: Record<string, Category>;
};
declare const lhqModelCategorySchema: z.ZodType<Category>;
declare const lhqModelUidSchema: z.ZodLiteral<"6ce4d54c5dbd415c93019d315e278638">;
declare const lhqModelVersionSchema: z.ZodUnion<[z.ZodLiteral<1>, z.ZodLiteral<2>]>;
declare const lhqModelCategoriesCollectionSchema: z.ZodRecord<z.ZodString, z.ZodType<Category, z.ZodTypeDef, Category>>;
declare const lhqModelResourcesCollectionSchema: z.ZodRecord<z.ZodString, z.ZodType<{
    values: Record<string, {
        value?: string | undefined;
        locked?: boolean | undefined;
        auto?: boolean | undefined;
    }>;
    state: "New" | "Edited" | "NeedsReview" | "Final";
    description?: string | undefined;
    parameters?: Record<string, {
        order: number;
        description?: string | undefined;
    }> | undefined;
}, z.ZodTypeDef, {
    values: Record<string, {
        value?: string | undefined;
        locked?: boolean | undefined;
        auto?: boolean | undefined;
    }>;
    state: "New" | "Edited" | "NeedsReview" | "Final";
    description?: string | undefined;
    parameters?: Record<string, {
        order: number;
        description?: string | undefined;
    }> | undefined;
}>>;
declare const lhqModelOptionsSchema: z.ZodObject<{
    categories: z.ZodBoolean;
    resources: z.ZodUnion<[z.ZodLiteral<"All">, z.ZodLiteral<"Categories">]>;
}, "strip", z.ZodTypeAny, {
    resources: "All" | "Categories";
    categories: boolean;
}, {
    resources: "All" | "Categories";
    categories: boolean;
}>;
declare const lqhModelMetadataSchema: z.ZodObject<{
    childs: z.ZodOptional<z.ZodArray<z.ZodType<BaseDataNode, z.ZodTypeDef, BaseDataNode>, "many">>;
}, "strip", z.ZodTypeAny, {
    childs?: BaseDataNode[] | undefined;
}, {
    childs?: BaseDataNode[] | undefined;
}>;
declare const lhqModelSchema: z.ZodObject<{
    model: z.ZodObject<{
        uid: z.ZodLiteral<"6ce4d54c5dbd415c93019d315e278638">;
        version: z.ZodUnion<[z.ZodLiteral<1>, z.ZodLiteral<2>]>;
        options: z.ZodObject<{
            categories: z.ZodBoolean;
            resources: z.ZodUnion<[z.ZodLiteral<"All">, z.ZodLiteral<"Categories">]>;
        }, "strip", z.ZodTypeAny, {
            resources: "All" | "Categories";
            categories: boolean;
        }, {
            resources: "All" | "Categories";
            categories: boolean;
        }>;
        name: z.ZodString;
        description: z.ZodOptional<z.ZodString>;
        primaryLanguage: z.ZodString;
    }, "strip", z.ZodTypeAny, {
        options: {
            resources: "All" | "Categories";
            categories: boolean;
        };
        name: string;
        uid: "6ce4d54c5dbd415c93019d315e278638";
        version: 1 | 2;
        primaryLanguage: string;
        description?: string | undefined;
    }, {
        options: {
            resources: "All" | "Categories";
            categories: boolean;
        };
        name: string;
        uid: "6ce4d54c5dbd415c93019d315e278638";
        version: 1 | 2;
        primaryLanguage: string;
        description?: string | undefined;
    }>;
    languages: z.ZodArray<z.ZodString, "many">;
    metadatas: z.ZodOptional<z.ZodObject<{
        childs: z.ZodOptional<z.ZodArray<z.ZodType<BaseDataNode, z.ZodTypeDef, BaseDataNode>, "many">>;
    }, "strip", z.ZodTypeAny, {
        childs?: BaseDataNode[] | undefined;
    }, {
        childs?: BaseDataNode[] | undefined;
    }>>;
    resources: z.ZodOptional<z.ZodLazy<z.ZodRecord<z.ZodString, z.ZodType<{
        values: Record<string, {
            value?: string | undefined;
            locked?: boolean | undefined;
            auto?: boolean | undefined;
        }>;
        state: "New" | "Edited" | "NeedsReview" | "Final";
        description?: string | undefined;
        parameters?: Record<string, {
            order: number;
            description?: string | undefined;
        }> | undefined;
    }, z.ZodTypeDef, {
        values: Record<string, {
            value?: string | undefined;
            locked?: boolean | undefined;
            auto?: boolean | undefined;
        }>;
        state: "New" | "Edited" | "NeedsReview" | "Final";
        description?: string | undefined;
        parameters?: Record<string, {
            order: number;
            description?: string | undefined;
        }> | undefined;
    }>>>>;
    categories: z.ZodOptional<z.ZodLazy<z.ZodRecord<z.ZodString, z.ZodType<Category, z.ZodTypeDef, Category>>>>;
}, "strip", z.ZodTypeAny, {
    model: {
        options: {
            resources: "All" | "Categories";
            categories: boolean;
        };
        name: string;
        uid: "6ce4d54c5dbd415c93019d315e278638";
        version: 1 | 2;
        primaryLanguage: string;
        description?: string | undefined;
    };
    languages: string[];
    resources?: Record<string, {
        values: Record<string, {
            value?: string | undefined;
            locked?: boolean | undefined;
            auto?: boolean | undefined;
        }>;
        state: "New" | "Edited" | "NeedsReview" | "Final";
        description?: string | undefined;
        parameters?: Record<string, {
            order: number;
            description?: string | undefined;
        }> | undefined;
    }> | undefined;
    categories?: Record<string, Category> | undefined;
    metadatas?: {
        childs?: BaseDataNode[] | undefined;
    } | undefined;
}, {
    model: {
        options: {
            resources: "All" | "Categories";
            categories: boolean;
        };
        name: string;
        uid: "6ce4d54c5dbd415c93019d315e278638";
        version: 1 | 2;
        primaryLanguage: string;
        description?: string | undefined;
    };
    languages: string[];
    resources?: Record<string, {
        values: Record<string, {
            value?: string | undefined;
            locked?: boolean | undefined;
            auto?: boolean | undefined;
        }>;
        state: "New" | "Edited" | "NeedsReview" | "Final";
        description?: string | undefined;
        parameters?: Record<string, {
            order: number;
            description?: string | undefined;
        }> | undefined;
    }> | undefined;
    categories?: Record<string, Category> | undefined;
    metadatas?: {
        childs?: BaseDataNode[] | undefined;
    } | undefined;
}>;
type LhqModelUid = z.infer<typeof lhqModelUidSchema>;
type LhqModelVersion = z.infer<typeof lhqModelVersionSchema>;
type LhqModelOptions = z.infer<typeof lhqModelOptionsSchema>;
type LhqModelOptionsResources = z.infer<typeof lhqModelOptionsResourcesSchema>;
type LhqModelResourceParameter = z.infer<typeof lhqModelResourceParameterSchema>;
type LhqModelResourceTranslationState = z.infer<typeof lhqModelResourceTranslationStateSchema>;
type LhqModelResourceValue = z.infer<typeof lhqModelResourceValueScheme>;
type LhqModelResource = z.infer<typeof lhqModelResourceSchema>;
type LhqModelCategory = z.infer<typeof lhqModelCategorySchema>;
type LhqModelCategoriesCollection = z.infer<typeof lhqModelCategoriesCollectionSchema>;
type LhqModelResourcesCollection = z.infer<typeof lhqModelResourcesCollectionSchema>;
type LhqModelDataNode = z.infer<typeof lqhModelDataNodeSchema>;
type LqhModelMetadata = z.infer<typeof lqhModelMetadataSchema>;
type LhqModel = z.infer<typeof lhqModelSchema>;

export { type LhqModel, type LhqModelCategoriesCollection, type LhqModelCategory, type LhqModelDataNode, type LhqModelOptions, type LhqModelOptionsResources, type LhqModelResource, type LhqModelResourceParameter, type LhqModelResourceTranslationState, type LhqModelResourceValue, type LhqModelResourcesCollection, type LhqModelUid, type LhqModelVersion, type LqhModelMetadata, lhqModelCategoriesCollectionSchema, lhqModelCategorySchema, lhqModelOptionsResourcesSchema, lhqModelOptionsSchema, lhqModelResourceParameterSchema, lhqModelResourceSchema, lhqModelResourceSchemaBase, lhqModelResourceTranslationStateSchema, lhqModelResourceValueScheme, lhqModelResourcesCollectionSchema, lhqModelSchema, lhqModelUidSchema, lhqModelVersionSchema, lqhModelDataNodeSchema, lqhModelMetadataSchema };
