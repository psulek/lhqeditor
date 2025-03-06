import { z } from "zod";

export const LhqModelOptionsResourcesSchema = z.union([
    z.literal("All"), z.literal("Categories")
]);

export const LhqModelResourceParameterSchema = z.object({
    description: z.string().optional(),
    order: z.number()
});

export const LhqModelResourceTranslationStateSchema = z.union([
    z.literal("New"), z.literal("Edited"), z.literal("NeedsReview"), z.literal("Final")
]);

export const LhqModelResourceValueSchema = z.object({
    value: z.string().optional(),
    locked: z.boolean().optional(),
    auto: z.boolean().optional()
});

export const LhqModelResourceSchemaBase = z.object({
    state: LhqModelResourceTranslationStateSchema,
    description: z.string().optional(),
    parameters: z.record(LhqModelResourceParameterSchema).optional(),
    values: z.record(LhqModelResourceValueSchema)
});

type Resource = z.infer<typeof LhqModelResourceSchemaBase> & {};

export const LhqModelResourceSchema: z.ZodType<Resource> = LhqModelResourceSchemaBase;


export const baseDataNodeSchema = z.object({
    name: z.string(),
    attrs: z.record(z.string()).optional()
});

type BaseDataNode = z.infer<typeof baseDataNodeSchema> & {
    childs?: BaseDataNode[];
}

export const LhqModelDataNodeSchema: z.ZodType<BaseDataNode> = baseDataNodeSchema.extend({
    childs: z.lazy(() => z.array(LhqModelDataNodeSchema)).optional()
});

export const baseCategorySchema = z.object({
    description: z.string().optional(),
    resources: z.lazy(() => LhqModelResourcesCollectionSchema).optional()
});

type Category = z.infer<typeof baseCategorySchema> & {
    categories?: Record<string, Category>;
};

export const LhqModelCategorySchema: z.ZodType<Category> = baseCategorySchema.extend({
    categories: z.lazy(() => LhqModelCategoriesCollectionSchema).optional()
});

export const LhqModelUidSchema = z.literal('6ce4d54c5dbd415c93019d315e278638');

export const LhqModelVersionSchema = z.union([z.literal(1), z.literal(2)]);

export const LhqCodeGenVersionSchema = z.literal(1);

export const LhqModelCategoriesCollectionSchema = z.record(LhqModelCategorySchema);

export const LhqModelResourcesCollectionSchema = z.record(LhqModelResourceSchema);

export const LhqModelOptionsSchema = z.object({
    categories: z.boolean(),
    resources: LhqModelOptionsResourcesSchema
});

export const LhqModelMetadataSchema = z.object({
    childs: z.array(LhqModelDataNodeSchema).optional(),
});

export const LhqModelSchema = z.object({
    model: z.object({
        uid: LhqModelUidSchema,
        version: LhqModelVersionSchema,
        options: LhqModelOptionsSchema,
        name: z.string(),
        description: z.string().optional(),
        primaryLanguage: z.string()
    }),
    languages: z.array(z.string()),
    metadatas: LhqModelMetadataSchema.optional(),
    resources: z.lazy(() => LhqModelResourcesCollectionSchema).optional(),
    categories: z.lazy(() => LhqModelCategoriesCollectionSchema).optional()
});

export type LhqModelUid = z.infer<typeof LhqModelUidSchema>;

export type LhqModelVersion = z.infer<typeof LhqModelVersionSchema>;

export type LhqCodeGenVersion = z.infer<typeof LhqCodeGenVersionSchema>;

export type LhqModelOptions = z.infer<typeof LhqModelOptionsSchema>;

export type LhqModelOptionsResources = z.infer<typeof LhqModelOptionsResourcesSchema>;

export type LhqModelResourceParameter = z.infer<typeof LhqModelResourceParameterSchema>;

export type LhqModelResourceTranslationState = z.infer<typeof LhqModelResourceTranslationStateSchema>;

export type LhqModelResourceValue = z.infer<typeof LhqModelResourceValueSchema>;

export type LhqModelResource = z.infer<typeof LhqModelResourceSchema>;

export type LhqModelCategory = z.infer<typeof LhqModelCategorySchema>;

export type LhqModelCategoriesCollection = z.infer<typeof LhqModelCategoriesCollectionSchema>;

export type LhqModelResourcesCollection = z.infer<typeof LhqModelResourcesCollectionSchema>;

export type LhqModelDataNode = z.infer<typeof LhqModelDataNodeSchema>;

export type LhqModelMetadata = z.infer<typeof LhqModelMetadataSchema>;

export type LhqModel = z.infer<typeof LhqModelSchema>;