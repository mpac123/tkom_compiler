using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using TKOM.Exceptions;
using TKOM.Structures.AST;

namespace TKOM.Structures.IR
{
    public class Scope
    {
        public Scope(ScopePrototype scopePrototype)
        {
            ScopePrototype = scopePrototype;
            VariableValues = new Dictionary<string, AssignedValue>();
        }

        public Scope(ScopePrototype scopePrototype, Scope upperScope)
        {
            UpperScope = upperScope;
            ScopePrototype = scopePrototype;
            VariableValues = new Dictionary<string, AssignedValue>();
        }
        public Scope UpperScope { set; get; }
        public ScopePrototype ScopePrototype { private set; get; }
        public IDictionary<string, AssignedValue> VariableValues { set; get; }

        public AssignedValue FindValueOfVariable(string variableName)
        {
            Scope scope = this;
            AssignedValue result;
            while (scope != null)
            {
                if (scope.VariableValues.TryGetValue(variableName, out result))
                {
                    return result;
                }
                scope = scope.UpperScope;
            }
            throw new RuntimeException($"Value {variableName} could not be found");
        }

        public void Initialize(List<AssignedValue> arguments)
        {
            VariableValues = new Dictionary<string, AssignedValue>();
            if (arguments.Count() > ScopePrototype.Variables.Count())
            {
                throw new SemanticsException($"Tried to initialize scope with variables that were not declared.");
            }
            if (arguments.Count() < ScopePrototype.Variables.Count())
            {
                throw new SemanticsException($"Some of declared variables were not initialized.");
            }
            foreach (var variableName in ScopePrototype.Variables)
            {
                VariableValues.Add(variableName, arguments.First());
                arguments = arguments.Skip(1).ToList();
            }
        }

        public JToken FindValueOfValueOf(ValueOf valueOf)
        {
            var rootValue = FindValueOfVariable(valueOf.VariableName);
            var rootName = valueOf.VariableName;
            if (rootValue.IsNumericValue)
            {
                if (valueOf.Index != null || valueOf.NestedValue != null)
                {
                    throw new RuntimeException("Tried to access members of a numeric value.");
                }
                return rootValue.NumericValue.ToString();
            }
            var currentValueOf = valueOf;
            var alreadyParsedBuilder = new StringBuilder(rootName);
            JToken jToken = rootValue.StringValue;
            do
            {
                if (currentValueOf.Index != null)
                {
                    var list = jToken.ToList();
                    if (list.Count() <= currentValueOf.Index)
                    {
                        throw new RuntimeException($"The object {alreadyParsedBuilder.ToString()} does not have a member of index {currentValueOf.Index}.");
                    }
                    jToken = list[currentValueOf.Index.Value];
                    alreadyParsedBuilder.Append($"[{currentValueOf.Index}]");
                }
                currentValueOf = currentValueOf.NestedValue;
                if (currentValueOf != null)
                {
                    try
                    {
                        jToken = jToken[currentValueOf.VariableName];
                    }
                    catch (InvalidOperationException)
                    {
                        throw new RuntimeException($"The object {alreadyParsedBuilder.ToString()} does not have a member called {currentValueOf.VariableName}.");
                    }
                    alreadyParsedBuilder.Append($".{currentValueOf.VariableName}");
                }
            }
            while (currentValueOf != null);
            return jToken;
        }
    }
}