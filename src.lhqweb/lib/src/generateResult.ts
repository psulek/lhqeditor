import { GeneratedFile } from './generatedFile';
import { LhqModelGroupSettings } from './modelGroupSettings';

/**
 * Represents result of code generator process of Generator.generate.
 */
export class GenerateResult {
    /**
     * Initializes a new instance of the GenerateResult class.
     * @param generatedFiles List of generated files.
     * @param modelGroupSettings List of settings used in code generator process.
     */
    constructor(
        public generatedFiles: Readonly<GeneratedFile[]>,
        public modelGroupSettings: Readonly<LhqModelGroupSettings[]>
    ) {}
}