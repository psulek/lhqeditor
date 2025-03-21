import { DOMParser } from '@xmldom/xmldom';
import * as xpath from 'xpath';
import zodToJsonSchema from 'zod-to-json-schema';
import { fromZodError } from 'zod-validation-error';

import { LhqModel, LhqModelSchema } from './api/schemas';
import { isNullOrEmpty, tryRemoveBOM } from './utils';

export function validateLhqModel(data: LhqModel): { success: boolean, error: string | undefined, model?: LhqModel } {
    if (data === undefined || data === null || typeof data !== 'object') {
        return { success: false, error: 'File or model must be specified.' };
    }

    const parseResult = LhqModelSchema.safeParse(data);
    const success = parseResult.success;
    const error = parseResult.success ? undefined : fromZodError(parseResult.error).toString();
    return { success, error, model: success ? parseResult.data : undefined };
}

export function generateSchema(): string {
    const jsonSchema = zodToJsonSchema(LhqModelSchema, {
        name: 'LhqModel',
        $refStrategy: 'root'
    });

    return JSON.stringify(jsonSchema, null, 2);
}

// Constants and helper variables
const itemGroupTypes = ['None', 'Compile', 'Content', 'EmbeddedResource'];
const itemGroupTypesAttrs = ['Include', 'Update'];
const csProjectXPath = '//ns:ItemGroup/ns:##TYPE##[@##ATTR##="##FILE##"]';
const xpathRootNamespace = 'string(//ns:RootNamespace)';
const xpathAssemblyName = 'string(//ns:AssemblyName)';

export function getRootNamespace(lhqModelFileName: string, t4FileName: string, 
    csProjectFileName: string, csProjectFileContent: string): string | undefined {
    let referencedLhqFile = false;

    if (isNullOrEmpty(csProjectFileName) || isNullOrEmpty(csProjectFileContent)) {
        return undefined;
    }

    let rootNamespace: string | undefined;

    try {
        const fileContent = tryRemoveBOM(csProjectFileContent);
        const doc = new DOMParser().parseFromString(fileContent, 'text/xml');
        const rootNode = doc as unknown as Node;
        const ns = doc.documentElement?.namespaceURI || '';

        const xpathSelect = xpath.useNamespaces({ns: ns});

        function findFileElement(fileName: string): Element | undefined {
            for (const itemGroupType of itemGroupTypes) {
                for (const attr of itemGroupTypesAttrs) {
                    const xpathQuery = csProjectXPath.replace('##TYPE##', itemGroupType)
                        .replace('##ATTR##', attr)
                        .replace('##FILE##', fileName);

                    const element = xpathSelect(xpathQuery, rootNode, true) as Element;
                    if (element) {
                        return element;
                    }
                }
            }
            return undefined;
        }

        rootNamespace = xpathSelect(xpathRootNamespace, rootNode, true) as string;

        referencedLhqFile = findFileElement(lhqModelFileName) != undefined;
        const t4FileElement = findFileElement(t4FileName);
        if (t4FileElement) {
            const dependentUpon = t4FileElement.getElementsByTagNameNS(ns, 'DependentUpon')[0]?.textContent;
            if (dependentUpon && dependentUpon === lhqModelFileName) {
                referencedLhqFile = true;
            }

            const customToolNamespace = t4FileElement.getElementsByTagNameNS(ns, 'CustomToolNamespace')[0]?.textContent;
            if (customToolNamespace) {
                rootNamespace = customToolNamespace;
            }
        }

        if (!rootNamespace) {
            rootNamespace = xpathSelect(xpathAssemblyName, rootNode, true) as string;

            // if (!rootNamespace && csProjectFileName) {
            //     rootNamespace = path.basename(csProjectFileName, path.extname(csProjectFileName)).replace(' ', '_');
            // }
        }
    } catch (e) {
        console.error('Error getting root namespace.', e);
        rootNamespace = undefined;
    }

    return rootNamespace;
}