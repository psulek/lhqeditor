/**
 * Model group settings used in code generator process of Generator.generate.
 */
export class LhqModelGroupSettings {
    private _settings: Map<string, any>;

    /**
     * Initializes a new instance of the LhqModelGroupSettings class.
     * @param group Settings group name
     */
    constructor(public group: string) {
        this._settings = new Map<string, any>();
    }

    /**
     * Get settings dictionary.
     */
    public get settings(): Readonly<Map<string, any>> {
        return this._settings
    }
}