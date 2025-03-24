import type { LhqModelLineEndings } from '../api/schemas.mjs';
import type { CodeGeneratorBasicSettings, ModelVersionsType } from '../api/modelTypes.mjs';

export const ModelVersions: ModelVersionsType = Object.freeze<ModelVersionsType>({
    model: 2,
    codeGenerator: 1
})

export const DefaultLineEndings: LhqModelLineEndings = 'LF';

export const DefaultCodeGenSettings: CodeGeneratorBasicSettings = {
    OutputFolder: 'Resources',
    EncodingWithBOM: false,
    LineEndings: DefaultLineEndings,
    Enabled: true
};