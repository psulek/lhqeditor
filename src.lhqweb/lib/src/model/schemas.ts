import { z } from "zod";

export const lhqModelOptionsResourcesSchema = z.union([
    z.literal("All"), z.literal("Categories")
]);

export const lhqModelResourceParameterSchema = z.object({
    description: z.string().optional(),
    order: z.number()
});

export const lhqModelResourceTranslationStateSchema = z.union([
    z.literal("New"), z.literal("Edited"), z.literal("NeedsReview"), z.literal("Final")
]);

export const lhqModelResourceValueScheme = z.object({
    value: z.string().optional(),
    locked: z.boolean().optional(),
    auto: z.boolean().optional()
});

export const lhqModelResourceSchemaBase = z.object({
    state: lhqModelResourceTranslationStateSchema,
    description: z.string().optional(),
    parameters: z.record(lhqModelResourceParameterSchema).optional(),
    values: z.record(lhqModelResourceValueScheme)
});

type Resource = z.infer<typeof lhqModelResourceSchemaBase> & {};

export const lhqModelResourceSchema: z.ZodType<Resource> = lhqModelResourceSchemaBase;


const baseDataNodeSchema = z.object({
    name: z.string(),
    attrs: z.record(z.string()).optional()
});

type BaseDataNode = z.infer<typeof baseDataNodeSchema> & {
    childs?: BaseDataNode[];
}

export const lqhModelDataNodeSchema: z.ZodType<BaseDataNode> = baseDataNodeSchema.extend({
    childs: z.lazy(() => z.array(lqhModelDataNodeSchema)).optional()
});

const baseCategorySchema = z.object({
    description: z.string().optional(),
    //resources: z.record(lhqModelResourceSchema).optional()
    resources: z.lazy(() => lhqModelResourcesCollectionSchema).optional()
});

type Category = z.infer<typeof baseCategorySchema> & {
    categories?: Record<string, Category>;
};

export const lhqModelCategorySchema: z.ZodType<Category> = baseCategorySchema.extend({
    //categories: z.lazy(() => z.record(lhqModelCategorySchema)).optional()
    categories: z.lazy(() => lhqModelCategoriesCollectionSchema).optional()
});

export const lhqModelUidSchema = z.literal('6ce4d54c5dbd415c93019d315e278638');

export const lhqModelVersionSchema = z.union([z.literal(1), z.literal(2)]);

export const lhqModelCategoriesCollectionSchema = z.record(lhqModelCategorySchema);

export const lhqModelResourcesCollectionSchema = z.record(lhqModelResourceSchema);

export const lhqModelOptionsSchema = z.object({
    categories: z.boolean(),
    resources: lhqModelOptionsResourcesSchema
});

export const lqhModelMetadataSchema = z.object({
    childs: z.array(lqhModelDataNodeSchema).optional(),
});

export const lhqModelSchema = z.object({
    model: z.object({
        uid: lhqModelUidSchema,
        version: lhqModelVersionSchema,
        options: lhqModelOptionsSchema,
        name: z.string(),
        description: z.string().optional(),
        primaryLanguage: z.string()
    }),
    languages: z.array(z.string()),
    metadatas: lqhModelMetadataSchema.optional(),
    resources: z.lazy(() => lhqModelResourcesCollectionSchema).optional(),
    categories: z.lazy(() => lhqModelCategoriesCollectionSchema).optional()
});


export type LhqModelUid = z.infer<typeof lhqModelUidSchema>;

export type LhqModelVersion = z.infer<typeof lhqModelVersionSchema>;

export type LhqModelOptions = z.infer<typeof lhqModelOptionsSchema>;

export type LhqModelOptionsResources = z.infer<typeof lhqModelOptionsResourcesSchema>;

export type LhqModelResourceParameter = z.infer<typeof lhqModelResourceParameterSchema>;

export type LhqModelResourceTranslationState = z.infer<typeof lhqModelResourceTranslationStateSchema>;

export type LhqModelResourceValue = z.infer<typeof lhqModelResourceValueScheme>;

export type LhqModelResource = z.infer<typeof lhqModelResourceSchema>;

export type LhqModelCategory = z.infer<typeof lhqModelCategorySchema>;

export type LhqModelCategoriesCollection = z.infer<typeof lhqModelCategoriesCollectionSchema>;

export type LhqModelResourcesCollection = z.infer<typeof lhqModelResourcesCollectionSchema>;

export type LhqModelDataNode = z.infer<typeof lqhModelDataNodeSchema>;

export type LqhModelMetadata = z.infer<typeof lqhModelMetadataSchema>;

export type LhqModel = z.infer<typeof lhqModelSchema>;