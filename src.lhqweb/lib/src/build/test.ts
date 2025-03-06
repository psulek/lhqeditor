import { LhqModel } from '../model/api/schemas';
import { validateLhqModel } from './utils';

const model = getModel();
const result = validateLhqModel(model);
console.log(`Model is valid: ${result.success}${result.success ? '' : `, ${result.error}`}`);


function getModel(): LhqModel {
    const model: LhqModel = {
        model: {
            name: "name",
            options: {
                categories: true,
                resources: 'Categories'
            },
            primaryLanguage: "en",
            uid: "6ce4d54c5dbd415c93019d315e278638",
            version: 2
        },
        languages: ["en", "sk"],
        metadatas: {
            childs: [
                {
                    name: "metadata",
                    attrs: {
                        descriptorUID: "b40c8a1d-23b7-4f78-991b-c24898596dd2"
                    },
                    childs: [
                        {
                            name: "content",
                            attrs: {
                                templateId: "WpfResxCsharp01"
                            },
                            childs: [
                                {
                                    name: "Settings",
                                    childs: [
                                        {
                                            name: "CSharp",
                                            attrs: {
                                                OutputFolder: "Resources",
                                                EncodingWithBOM: "false",
                                                LineEndings: "CRLF",
                                                UseExpressionBodySyntax: "true",
                                                MissingTranslationFallbackToPrimary: "true"
                                            }
                                        },
                                        {
                                            name: "ResX",
                                            attrs: {
                                                OutputFolder: "Resources",
                                                EncodingWithBOM: "false",
                                                LineEndings: "CRLF",
                                                CultureCodeInFileNameForPrimaryLanguage: "true"
                                            }
                                        }
                                    ]
                                }
                            ]
                        }
                    ]
                }
            ]
        },
        resources: {
            resource1: {
                state: "New",
                values: {
                    value1: {
                        value: "value1"
                    }
                }
            }
        },
        categories: {
            a: {
                categories: {
                    asdd: {

                    }
                },
                resources: {
                    a: {
                        state: "New",
                        values: {
                            value1: {
                                value: "value1"
                            }
                        }
                    }
                }
            }
        }
    }

    return model;
}