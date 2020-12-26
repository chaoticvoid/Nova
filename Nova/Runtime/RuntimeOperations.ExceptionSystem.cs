// -----------------------------------------------------------------------
// <copyright file="RuntimeOperations.ExceptionSystem.cs" Company="Michael Tindal">
// Copyright 2011-2014 Michael Tindal
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Nova.Expressions;
using Nova.Builtins;

namespace Nova.Runtime
{
    public static partial class RuntimeOperations
    {
        internal static dynamic Throw(Expression rawObjExpr, object rawScope)
        {
            var scope = rawScope as NovaScope ?? new NovaScope();
            var rawObj = CompilerServices.CompileExpression(rawObjExpr, scope);

            var obj = rawObj as NovaInstance;
            if (obj == null) throw new Exception();
            var instance = obj as NovaBoxedInstance;
            if (instance == null) throw new NovaException(obj);
            var exc = instance.BoxedObject as Exception;
            if (exc != null)
            {
                throw exc;
            }
            throw new Exception();
        }

        internal static dynamic Begin(object rawTryExpression, List<Expression> rescueBlocksRaw,
            object rawEnsureBlock, object rawElseBlock, object rawScope)
        {
            var tryExpression = (Expression) rawTryExpression;
            var ensureBlock = (Expression) rawEnsureBlock;
            var elseBlock = (Expression) rawElseBlock;
            var scope = (NovaScope) rawScope;
            dynamic retVal = null;
            var exceptionRaised = false;
            var ensureRun = false;
            var rescueBlocks = new List<RescueExpression>();
            rescueBlocksRaw.ForEach(
                rawBlock =>
                {
                    var block = rawBlock as RescueExpression;
                    if (block != null) rescueBlocks.Add(block);
                });

            try
            {
                retVal = CompilerServices.CompileExpression(tryExpression, scope);
            }
            catch (Exception e)
            {
                var NovaException = e as NovaException;
                var exType = NovaException != null ? NovaException.ExceptionClass.Name : e.GetType().Name;
                var found = false;
                exceptionRaised = true;
                foreach (var rescueBlock in rescueBlocks)
                {
                    var exceptionTypes = new List<string>();
                    if (!rescueBlock.IsWildcard)
                    {
                        foreach (var type in rescueBlock.ExceptionTypes)
                        {
                            var obj = Resolve(type, scope);
                            var instance = obj as NovaInstance;
                            if (instance != null)
                            {
                                exceptionTypes.Add(instance.Class.Name);
                            }
                            else
                            {
                                var @class = obj as NovaClass;
                                if (@class != null)
                                {
                                    exceptionTypes.Add(@class.Name);
                                }
                                var s = obj as string;
                                if (s != null)
                                {
                                    exceptionTypes.Add(s);
                                }
                                var ss = obj as NovaString;
                                if (ss != null)
                                {
                                    exceptionTypes.Add(ss);
                                }
                            }
                        }
                    }
                    var exMatches = rescueBlock.IsWildcard;
                    if (!exMatches)
                    {
                        if ((from type in exceptionTypes select NovaTypeResolver.Resolve(type) into _exType where _exType != null let __exType = NovaTypeResolver.Resolve(exType) where __exType != null && __exType.IsSubclassOf(_exType) || __exType == _exType select _exType).Any())
                        {
                            exMatches = true;
                        }
                    }
                    found = exMatches;
                    if (!found)
                    {
                        if (exceptionTypes.Contains(exType))
                        {
                            found = true;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    var exception = e as NovaException;
                    if (exception != null)
                    {
                        scope[rescueBlock.VarName] = exception.InnerObject;
                    }
                    else
                    {
                        scope[rescueBlock.VarName] = e;
                    }
                    try
                    {
                        retVal = CompilerServices.CompileExpression(rescueBlock.Body, scope);
                    }
                    catch (Exception)
                    {
                        if (ensureBlock == null) throw;
                        ensureRun = true;
                        CompilerServices.CompileExpression(ensureBlock, scope);
                        throw;
                    }
                    break;
                }
                if (!found)
                {
                    throw;
                }
            }
            finally
            {
                if (!exceptionRaised && elseBlock != null)
                {
                    CompilerServices.CompileExpression(elseBlock, scope);
                }
                if (!ensureRun && ensureBlock != null)
                {
                    CompilerServices.CompileExpression(ensureBlock, scope);
                }
            }
            return retVal;
        }
    }
}
