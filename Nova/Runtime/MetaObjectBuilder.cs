//

using System.Dynamic;
using System.Linq.Expressions;
using Nova.Expressions;

//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
// -----------------------------------------------------------------------

namespace Nova.Runtime {
    internal class MetaObjectBuilder {
        private Expression _metaResult;
        private BindingRestrictions _restrictions;

        public MetaObjectBuilder(NovaMetaObject target, DynamicMetaObject[] args) {
            _restrictions = null;
            if (target.Restrictions != BindingRestrictions.Empty) {
                _restrictions = target.Restrictions;
            }
        }

        protected void AddRestrictions(DynamicMetaObject[] args) {
            foreach (var arg in args) {
                if (arg.Restrictions != BindingRestrictions.Empty) {
                    AddRestriction(arg.Restrictions);
                }
                else {
                    // Runtime check!
                    AddRestriction(
                        BindingRestrictions.GetExpressionRestriction(NovaExpression.Binary(arg.Expression,
                            Expression.Constant(arg.Value), ExpressionType.Equal)));
                }
            }
        }

        public void SetMetaResult(Expression result) {
            _metaResult = result;
        }

        public void AddRestriction(BindingRestrictions restriction) {
            if (_restrictions == null) {
                _restrictions = restriction;
            }
            else {
                _restrictions.Merge(restriction);
            }
        }

        public DynamicMetaObject CreateMetaObject(IInteropBinder binder) {
            return new DynamicMetaObject(_metaResult, _restrictions);
        }
    }
}