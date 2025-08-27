using FluentValidation.TestHelper;
using ToggleHub.Application.DTOs.Flag.Create;
using ToggleHub.Application.Validators.Flag.Create;
using ToggleHub.Domain.Entities;

namespace ToggleHub.Application.UnitTests.Validators;

[TestFixture]
public class CreateFlagValidatorTests
{
    private CreateFlagValidator _validator;

    [SetUp]
    public void SetUp()
    {
        _validator = new CreateFlagValidator();
    }

    [Test]
    public void Validate_WithValidFlag_ShouldPass()
    {
        // Arrange
        var validFlag = new CreateFlagDto
        {
            ProjectId = 1,
            EnvironmentId = 1,
            Key = "test-flag",
            Description = "A test flag",
            Enabled = true,
            ReturnValueTypeString = "Boolean",
            DefaultValueOnRaw = "true",
            DefaultValueOffRaw = "false",
            RuleSets = new List<CreateRuleSetDto>()
        };

        // Act
        var result = _validator.TestValidate(validFlag);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void Validate_WithInvalidProjectId_ShouldFail()
    {
        // Arrange
        var invalidFlag = new CreateFlagDto
        {
            ProjectId = 0,
            EnvironmentId = 1,
            Key = "test-flag",
            RuleSets = new List<CreateRuleSetDto>()
        };

        // Act
        var result = _validator.TestValidate(invalidFlag);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ProjectId);
    }

    [Test]
    public void Validate_WithInvalidEnvironmentId_ShouldFail()
    {
        // Arrange
        var invalidFlag = new CreateFlagDto
        {
            ProjectId = 1,
            EnvironmentId = -1,
            Key = "test-flag",
            RuleSets = new List<CreateRuleSetDto>()
        };

        // Act
        var result = _validator.TestValidate(invalidFlag);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.EnvironmentId);
    }

    [Test]
    public void Validate_WithEmptyKey_ShouldFail()
    {
        // Arrange
        var invalidFlag = new CreateFlagDto
        {
            ProjectId = 1,
            EnvironmentId = 1,
            Key = "",
            RuleSets = new List<CreateRuleSetDto>()
        };

        // Act
        var result = _validator.TestValidate(invalidFlag);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Key);
    }

    [Test]
    public void Validate_WithNullKey_ShouldFail()
    {
        // Arrange
        var invalidFlag = new CreateFlagDto
        {
            ProjectId = 1,
            EnvironmentId = 1,
            Key = null!,
            RuleSets = new List<CreateRuleSetDto>()
        };

        // Act
        var result = _validator.TestValidate(invalidFlag);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Key);
    }

    [Test]
    public void Validate_WithKeyTooLong_ShouldFail()
    {
        // Arrange
        var invalidFlag = new CreateFlagDto
        {
            ProjectId = 1,
            EnvironmentId = 1,
            Key = new string('a', 101),
            RuleSets = new List<CreateRuleSetDto>()
        };

        // Act
        var result = _validator.TestValidate(invalidFlag);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Key);
    }

    [Test]
    public void Validate_WithInvalidKeyCharacters_ShouldFail()
    {
        // Arrange
        var invalidKeys = new[] { "test@flag", "test#flag", "test flag", "test.flag" };

        foreach (var invalidKey in invalidKeys)
        {
            var invalidFlag = new CreateFlagDto
            {
                ProjectId = 1,
                EnvironmentId = 1,
                Key = invalidKey,
                RuleSets = new List<CreateRuleSetDto>()
            };

            // Act
            var result = _validator.TestValidate(invalidFlag);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Key);
        }
    }

    [Test]
    public void Validate_WithValidKeyCharacters_ShouldPass()
    {
        // Arrange
        var validKeys = new[] { "test-flag", "test_flag", "testFlag", "test123", "TEST-FLAG" };

        foreach (var validKey in validKeys)
        {
            var validFlag = new CreateFlagDto
            {
                ProjectId = 1,
                EnvironmentId = 1,
                Key = validKey,
                RuleSets = new List<CreateRuleSetDto>()
            };

            // Act
            var result = _validator.TestValidate(validFlag);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Key);
        }
    }

    [Test]
    public void Validate_WithDescriptionTooLong_ShouldFail()
    {
        // Arrange
        var invalidFlag = new CreateFlagDto
        {
            ProjectId = 1,
            EnvironmentId = 1,
            Key = "test-flag",
            Description = new string('a', 1001),
            RuleSets = new List<CreateRuleSetDto>()
        };

        // Act
        var result = _validator.TestValidate(invalidFlag);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Test]
    public void Validate_WithValidDescription_ShouldPass()
    {
        // Arrange
        var validFlag = new CreateFlagDto
        {
            ProjectId = 1,
            EnvironmentId = 1,
            Key = "test-flag",
            Description = "A valid description",
            RuleSets = new List<CreateRuleSetDto>()
        };

        // Act
        var result = _validator.TestValidate(validFlag);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Test]
    public void Validate_WithEmptyDescription_ShouldPass()
    {
        // Arrange
        var validFlag = new CreateFlagDto
        {
            ProjectId = 1,
            EnvironmentId = 1,
            Key = "test-flag",
            Description = "",
            RuleSets = new List<CreateRuleSetDto>()
        };

        // Act
        var result = _validator.TestValidate(validFlag);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Test]
    public void Validate_WithValidRuleSets_ShouldPass()
    {
        // Arrange
        var validFlag = new CreateFlagDto
        {
            ProjectId = 1,
            EnvironmentId = 1,
            Key = "test-flag",
            RuleSets = new List<CreateRuleSetDto>
            {
                new CreateRuleSetDto
                {
                    Priority = 1,
                    Percentage = 100,
                    ReturnValueRaw = "true",
                    OffReturnValueRaw = "false",
                    Conditions = new List<CreateRuleConditionDto>()
                }
            }
        };

        // Act
        var result = _validator.TestValidate(validFlag);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void Validate_WithInvalidRuleSetPriority_ShouldFail()
    {
        // Arrange
        var invalidFlag = new CreateFlagDto
        {
            ProjectId = 1,
            EnvironmentId = 1,
            Key = "test-flag",
            RuleSets = new List<CreateRuleSetDto>
            {
                new CreateRuleSetDto
                {
                    Priority = 0,
                    Percentage = 100,
                    ReturnValueRaw = "true",
                    OffReturnValueRaw = "false",
                    Conditions = new List<CreateRuleConditionDto>()
                }
            }
        };

        // Act
        var result = _validator.TestValidate(invalidFlag);

        // Assert
        result.ShouldHaveValidationErrorFor("RuleSets[0].Priority");
    }

    [Test]
    public void Validate_WithInvalidRuleSetPercentage_ShouldFail()
    {
        // Arrange
        var invalidFlag = new CreateFlagDto
        {
            ProjectId = 1,
            EnvironmentId = 1,
            Key = "test-flag",
            RuleSets = new List<CreateRuleSetDto>
            {
                new CreateRuleSetDto
                {
                    Priority = 1,
                    Percentage = 101,
                    ReturnValueRaw = "true",
                    OffReturnValueRaw = "false",
                    Conditions = new List<CreateRuleConditionDto>()
                }
            }
        };

        // Act
        var result = _validator.TestValidate(invalidFlag);

        // Assert
        result.ShouldHaveValidationErrorFor("RuleSets[0].Percentage");
    }
    

    [Test]
    public void Validate_WithValidRuleConditions_ShouldPass()
    {
        // Arrange
        var validFlag = new CreateFlagDto
        {
            ProjectId = 1,
            EnvironmentId = 1,
            Key = "test-flag",
            RuleSets = new List<CreateRuleSetDto>
            {
                new CreateRuleSetDto
                {
                    Priority = 1,
                    Percentage = 100,
                    ReturnValueRaw = "true",
                    OffReturnValueRaw = "false",
                    Conditions = new List<CreateRuleConditionDto>
                    {
                        new CreateRuleConditionDto
                        {
                            Field = "userId",
                            FieldTypeString = "String",
                            OperatorString = "Equals",
                            ValueString = "123"
                        }
                    }
                }
            }
        };

        // Act
        var result = _validator.TestValidate(validFlag);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void Validate_WithInvalidRuleConditionField_ShouldFail()
    {
        // Arrange
        var invalidFlag = new CreateFlagDto
        {
            ProjectId = 1,
            EnvironmentId = 1,
            Key = "test-flag",
            RuleSets = new List<CreateRuleSetDto>
            {
                new CreateRuleSetDto
                {
                    Priority = 1,
                    Percentage = 100,
                    ReturnValueRaw = "true",
                    OffReturnValueRaw = "false",
                    Conditions = new List<CreateRuleConditionDto>
                    {
                        new CreateRuleConditionDto
                        {
                            Field = "",
                            FieldTypeString = "String",
                            OperatorString = "Equals",
                            ValueString = "123"
                        }
                    }
                }
            }
        };

        // Act
        var result = _validator.TestValidate(invalidFlag);

        // Assert
        result.ShouldHaveValidationErrorFor("RuleSets[0].Conditions[0].Field");
    }

    [Test]
    public void Validate_WithInvalidRuleConditionFieldType_ShouldFail()
    {
        // Arrange
        var invalidFlag = new CreateFlagDto
        {
            ProjectId = 1,
            EnvironmentId = 1,
            Key = "test-flag",
            RuleSets = new List<CreateRuleSetDto>
            {
                new CreateRuleSetDto
                {
                    Priority = 1,
                    Percentage = 100,
                    ReturnValueRaw = "true",
                    OffReturnValueRaw = "false",
                    Conditions = new List<CreateRuleConditionDto>
                    {
                        new CreateRuleConditionDto
                        {
                            Field = "userId",
                            FieldTypeString = "InvalidType",
                            OperatorString = "Equals",
                            ValueString = "123"
                        }
                    }
                }
            }
        };

        // Act
        var result = _validator.TestValidate(invalidFlag);

        // Assert
        result.ShouldHaveValidationErrorFor("RuleSets[0].Conditions[0].FieldType");
    }

    [Test]
    public void Validate_WithInvalidRuleConditionOperator_ShouldFail()
    {
        // Arrange
        var invalidFlag = new CreateFlagDto
        {
            ProjectId = 1,
            EnvironmentId = 1,
            Key = "test-flag",
            RuleSets = new List<CreateRuleSetDto>
            {
                new CreateRuleSetDto
                {
                    Priority = 1,
                    Percentage = 100,
                    ReturnValueRaw = "true",
                    OffReturnValueRaw = "false",
                    Conditions = new List<CreateRuleConditionDto>
                    {
                        new CreateRuleConditionDto
                        {
                            Field = "userId",
                            FieldTypeString = "String",
                            OperatorString = "InvalidOperator",
                            ValueString = "123"
                        }
                    }
                }
            }
        };

        // Act
        var result = _validator.TestValidate(invalidFlag);

        // Assert
        result.ShouldHaveValidationErrorFor("RuleSets[0].Conditions[0].Operator");
    }

    [Test]
    public void Validate_WithIncompatibleOperatorForFieldType_ShouldFail()
    {
        // Arrange
        var invalidFlag = new CreateFlagDto
        {
            ProjectId = 1,
            EnvironmentId = 1,
            Key = "test-flag",
            RuleSets = new List<CreateRuleSetDto>
            {
                new CreateRuleSetDto
                {
                    Priority = 1,
                    Percentage = 100,
                    ReturnValueRaw = "true",
                    OffReturnValueRaw = "false",
                    Conditions = new List<CreateRuleConditionDto>
                    {
                        new CreateRuleConditionDto
                        {
                            Field = "userId",
                            FieldTypeString = "String",
                            OperatorString = "GreaterThan",
                            ValueString = "123"
                        }
                    }
                }
            }
        };

        // Act
        var result = _validator.TestValidate(invalidFlag);

        // Assert
        result.ShouldHaveValidationErrorFor("RuleSets[0].Conditions[0]");
    }

    [Test]
    public void Validate_WithMissingStringValue_ShouldFail()
    {
        // Arrange
        var invalidFlag = new CreateFlagDto
        {
            ProjectId = 1,
            EnvironmentId = 1,
            Key = "test-flag",
            RuleSets = new List<CreateRuleSetDto>
            {
                new CreateRuleSetDto
                {
                    Priority = 1,
                    Percentage = 100,
                    ReturnValueRaw = "true",
                    OffReturnValueRaw = "false",
                    Conditions = new List<CreateRuleConditionDto>
                    {
                        new CreateRuleConditionDto
                        {
                            Field = "userId",
                            FieldTypeString = "String",
                            OperatorString = "Equals",
                            ValueString = ""
                        }
                    }
                }
            }
        };

        // Act
        var result = _validator.TestValidate(invalidFlag);

        // Assert
        result.ShouldHaveValidationErrorFor("RuleSets[0].Conditions[0].ValueString");
    }

    [Test]
    public void Validate_WithMissingNumberValue_ShouldFail()
    {
        // Arrange
        var invalidFlag = new CreateFlagDto
        {
            ProjectId = 1,
            EnvironmentId = 1,
            Key = "test-flag",
            RuleSets = new List<CreateRuleSetDto>
            {
                new CreateRuleSetDto
                {
                    Priority = 1,
                    Percentage = 100,
                    ReturnValueRaw = "true",
                    OffReturnValueRaw = "false",
                    Conditions = new List<CreateRuleConditionDto>
                    {
                        new CreateRuleConditionDto
                        {
                            Field = "userId",
                            FieldTypeString = "Number",
                            OperatorString = "Equals",
                            ValueNumber = null
                        }
                    }
                }
            }
        };

        // Act
        var result = _validator.TestValidate(invalidFlag);

        // Assert
        result.ShouldHaveValidationErrorFor("RuleSets[0].Conditions[0].ValueNumber");
    }

    [Test]
    public void Validate_WithMissingBooleanValue_ShouldFail()
    {
        // Arrange
        var invalidFlag = new CreateFlagDto
        {
            ProjectId = 1,
            EnvironmentId = 1,
            Key = "test-flag",
            RuleSets = new List<CreateRuleSetDto>
            {
                new CreateRuleSetDto
                {
                    Priority = 1,
                    Percentage = 100,
                    ReturnValueRaw = "true",
                    OffReturnValueRaw = "false",
                    Conditions = new List<CreateRuleConditionDto>
                    {
                        new CreateRuleConditionDto
                        {
                            Field = "userId",
                            FieldTypeString = "Boolean",
                            OperatorString = "Equals",
                            ValueBoolean = null
                        }
                    }
                }
            }
        };

        // Act
        var result = _validator.TestValidate(invalidFlag);

        // Assert
        result.ShouldHaveValidationErrorFor("RuleSets[0].Conditions[0].ValueBoolean");
    }

    [Test]
    public void Validate_WithListFieldTypeWithoutItems_ShouldFail()
    {
        // Arrange
        var invalidFlag = new CreateFlagDto
        {
            ProjectId = 1,
            EnvironmentId = 1,
            Key = "test-flag",
            RuleSets = new List<CreateRuleSetDto>
            {
                new CreateRuleSetDto
                {
                    Priority = 1,
                    Percentage = 100,
                    ReturnValueRaw = "true",
                    OffReturnValueRaw = "false",
                    Conditions = new List<CreateRuleConditionDto>
                    {
                        new CreateRuleConditionDto
                        {
                            Field = "userRoles",
                            FieldTypeString = "List",
                            OperatorString = "In",
                            Items = new List<CreateRuleConditionItemDto>()
                        }
                    }
                }
            }
        };

        // Act
        var result = _validator.TestValidate(invalidFlag);

        // Assert
        result.ShouldHaveValidationErrorFor("RuleSets[0].Conditions[0].Items");
    }

    [Test]
    public void Validate_WithListFieldTypeWithValidItems_ShouldPass()
    {
        // Arrange
        var validFlag = new CreateFlagDto
        {
            ProjectId = 1,
            EnvironmentId = 1,
            Key = "test-flag",
            RuleSets = new List<CreateRuleSetDto>
            {
                new CreateRuleSetDto
                {
                    Priority = 1,
                    Percentage = 100,
                    ReturnValueRaw = "true",
                    OffReturnValueRaw = "false",
                    Conditions = new List<CreateRuleConditionDto>
                    {
                        new CreateRuleConditionDto
                        {
                            Field = "userRoles",
                            FieldTypeString = "List",
                            OperatorString = "In",
                            Items = new List<CreateRuleConditionItemDto>
                            {
                                new CreateRuleConditionItemDto
                                {
                                    ValueString = "admin"
                                }
                            }
                        }
                    }
                }
            }
        };

        // Act
        var result = _validator.TestValidate(validFlag);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void Validate_WithInvalidRuleConditionItem_ShouldFail()
    {
        // Arrange
        var invalidFlag = new CreateFlagDto
        {
            ProjectId = 1,
            EnvironmentId = 1,
            Key = "test-flag",
            RuleSets = new List<CreateRuleSetDto>
            {
                new CreateRuleSetDto
                {
                    Priority = 1,
                    Percentage = 100,
                    ReturnValueRaw = "true",
                    OffReturnValueRaw = "false",
                    Conditions = new List<CreateRuleConditionDto>
                    {
                        new CreateRuleConditionDto
                        {
                            Field = "userRoles",
                            FieldTypeString = "List",
                            OperatorString = "In",
                            Items = new List<CreateRuleConditionItemDto>
                            {
                                new CreateRuleConditionItemDto
                                {
                                    ValueString = "",
                                    ValueNumber = null
                                }
                            }
                        }
                    }
                }
            }
        };

        // Act
        var result = _validator.TestValidate(invalidFlag);

        // Assert
        result.ShouldHaveValidationErrorFor("RuleSets[0].Conditions[0].Items[0]");
    }

    [Test]
    public void Validate_WithRuleConditionItemStringTooLong_ShouldFail()
    {
        // Arrange
        var invalidFlag = new CreateFlagDto
        {
            ProjectId = 1,
            EnvironmentId = 1,
            Key = "test-flag",
            RuleSets = new List<CreateRuleSetDto>
            {
                new CreateRuleSetDto
                {
                    Priority = 1,
                    Percentage = 100,
                    ReturnValueRaw = "true",
                    OffReturnValueRaw = "false",
                    Conditions = new List<CreateRuleConditionDto>
                    {
                        new CreateRuleConditionDto
                        {
                            Field = "userRoles",
                            FieldTypeString = "List",
                            OperatorString = "In",
                            Items = new List<CreateRuleConditionItemDto>
                            {
                                new CreateRuleConditionItemDto
                                {
                                    ValueString = new string('a', 501)
                                }
                            }
                        }
                    }
                }
            }
        };

        // Act
        var result = _validator.TestValidate(invalidFlag);

        // Assert
        result.ShouldHaveValidationErrorFor("RuleSets[0].Conditions[0].Items[0].ValueString");
        result.ShouldHaveValidationErrorFor("RuleSets[0].Conditions[0].Items[0].ValueString")
            .WithErrorMessage("String value must not exceed 500 characters.");
    }

    [Test]
    public void Validate_WithComplexValidFlag_ShouldPass()
    {
        // Arrange
        var validFlag = new CreateFlagDto
        {
            ProjectId = 1,
            EnvironmentId = 1,
            Key = "feature-toggle",
            Description = "A complex feature toggle with multiple rule sets",
            Enabled = true,
            ReturnValueTypeString = "String",
            DefaultValueOnRaw = "enabled",
            DefaultValueOffRaw = "disabled",
            RuleSets = new List<CreateRuleSetDto>
            {
                new CreateRuleSetDto
                {
                    Priority = 1,
                    Percentage = 50,
                    ReturnValueRaw = "enabled",
                    OffReturnValueRaw = "disabled",
                    Conditions = new List<CreateRuleConditionDto>
                    {
                        new CreateRuleConditionDto
                        {
                            Field = "userType",
                            FieldTypeString = "String",
                            OperatorString = "Equals",
                            ValueString = "premium"
                        }
                    }
                },
                new CreateRuleSetDto
                {
                    Priority = 2,
                    Percentage = 100,
                    ReturnValueRaw = "enabled",
                    OffReturnValueRaw = "disabled",
                    Conditions = new List<CreateRuleConditionDto>
                    {
                        new CreateRuleConditionDto
                        {
                            Field = "userRoles",
                            FieldTypeString = "List",
                            OperatorString = "In",
                            Items = new List<CreateRuleConditionItemDto>
                            {
                                new CreateRuleConditionItemDto { ValueString = "admin" },
                                new CreateRuleConditionItemDto { ValueString = "moderator" }
                            }
                        }
                    }
                }
            }
        };

        // Act
        var result = _validator.TestValidate(validFlag);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
