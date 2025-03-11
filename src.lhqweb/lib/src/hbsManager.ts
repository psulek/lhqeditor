import Handlebars from 'handlebars';
import { AppError } from './AppError';
import { getKnownHelpers } from './helpers';


export class HbsTemplateManager {
    private static _sources: {
        [templateId: string]: string;
    };

    private static _compiled: {
        [templateId: string]: HandlebarsTemplateDelegate;
    };

    public static registerTemplate(templateId: string, handlebarContent: string): void {
        HbsTemplateManager._sources ??= {};
        HbsTemplateManager._sources[templateId] = handlebarContent;
    }

    public static hasTemplate(templateId: string): boolean {
        return HbsTemplateManager._sources.hasOwnProperty(templateId);
    }

    public static runTemplate(templateId: string, data: unknown): string {
        let compiled: HandlebarsTemplateDelegate;

        HbsTemplateManager._compiled ??= {};
        if (!HbsTemplateManager._compiled.hasOwnProperty(templateId)) {
            if (!HbsTemplateManager._sources.hasOwnProperty(templateId)) {
                throw new AppError(`Template with id '${templateId}' not found !`);
            }

            const source = HbsTemplateManager._sources[templateId];
            compiled = Handlebars.compile(source, { knownHelpers: getKnownHelpers() });

            HbsTemplateManager._compiled[templateId] = compiled;
        } else {
            compiled = HbsTemplateManager._compiled[templateId];
        }

        const result = compiled(data, {
            allowProtoPropertiesByDefault: true,
            allowProtoMethodsByDefault: true,
            allowCallsToHelperMissing: true
        });
        if (result.indexOf('¤') > -1) {
            // NOTE: special tag to remove one tab (decrease indent)
            return result.replace(/\t¤$/gm, '');
        }

        return result;
    }
}
