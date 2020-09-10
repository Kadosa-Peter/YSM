using System.Collections.Generic;
using System.Linq;

namespace Ysm.Data
{
    public class Categories
    {
        private readonly Schema _schema;

        public Categories(Schema schema)
        {
            _schema = schema;
        }

        public List<Category> Get()
        {
            return CategoryQueries.Get();
        }

        public List<Category> Get(string id)
        {
            return CategoryQueries.Get(id);
        }

        public void Move(Dictionary<string, string> categories)
        {
            if (categories.Count > 0)
            {
                CategoryQueries.Move(categories);
                _schema.MoveCategory(categories);
            }
        }

        public int GetState(string id)
        {
            return _schema.GetCategoryState(id);
        }

        public bool HasUnwatchedVideoOrCategory(string id)
        {
            return _schema.HasUnwatchedVideoOrCategory(id);
        }

        public bool HasChildren(string id)
        {
            return _schema.HasChildren(id);
        }

        public void Remove(List<string> ids)
        {
            if (ids.Any())
            {
                _schema.DeleteCategories(ids);

                CategoryQueries.Delete(ids);
            }
        }

        public void Remove()
        {
            _schema.DeleteCategories();

            CategoryQueries.Delete();
        }

        public void Update(Category category)
        {
            CategoryQueries.Update(category);
        }

        public void Update(List<Category> categories)
        {
            CategoryQueries.Update(categories);
        }

        public void Insert(Category category)
        {
            _schema.Insert(category);

            CategoryQueries.Insert(category);
        }

        public void Insert(List<Category> categories)
        {
            _schema.Insert(categories);

            CategoryQueries.Insert(categories);
        }
    }
}
