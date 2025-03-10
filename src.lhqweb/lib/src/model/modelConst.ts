import { LhqModelCodeGeneratorBasicSettings, LhqModelLineEndings } from './api/schemas';
import { ModelVersionsType } from './api/types';

export const ModelVersions: ModelVersionsType = Object.freeze<ModelVersionsType>({
    model: 2,
    codeGenerator: 1
})

export const DefaultLineEndings: LhqModelLineEndings = 'lf';

export const DefaultCodeGenSettings: LhqModelCodeGeneratorBasicSettings = {
    OutputFolder: 'Resources',
    EncodingWithBOM: 'false',
    LineEndings: DefaultLineEndings
};