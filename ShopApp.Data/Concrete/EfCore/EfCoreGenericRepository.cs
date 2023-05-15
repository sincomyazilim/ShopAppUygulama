using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ShopApp.data.Abstract;

namespace ShopApp.data.Concrete.EfCore
{
    public class EfCoreGenericRepository<TEntity, TContext> : IRepository<TEntity>
    where TEntity : class                   //where ile yazılan yerler yazılmaya bılır sadece emın olmak ıcın TEntity ıstedıgı zaman class gondrecez TContext ıstedıgı zaman da dbcontext bılgısı gondermemsız gerekrı
    where TContext : DbContext, new()       //burda ekle update ıd getır vb metodlar aynı iş yaptıgı ıcın bunlarıda tek yerden doldurmak ıcın generıc class mantıgıyla kuruldu
    {
        public void Create(TEntity entity)
        {
            using (var context=new TContext())
            {
                context.Set<TEntity>().Add(entity);
                context.SaveChanges();
            }
        }

        public void Delete(TEntity entity)
        {
            using (var context=new TContext())
            {
                context.Set<TEntity>().Remove(entity);
                context.SaveChanges();
            }
        }

        public List<TEntity> GetAll()
        {
            using (var context=new TContext())
            {
                return context.Set<TEntity>().ToList();
            }
        }

        public TEntity GetById(int id)
        {
             using (var context=new TContext())
            {
                return context.Set<TEntity>().Find(id);
            }
        }

        public virtual void Update(TEntity entity)//virtual yaparak bu metotdu overide edebılıyurz ezıyorz yanı burdan efcorecartreposıtorde ezıyoruz
        {
             using (var context=new TContext())
            {
                context.Entry(entity).State=EntityState.Modified;//burda form uzerınde gucelleme yaptıgımızda hangı alanlar degıstıgını otomatık anlıyor ve ona gore degısen kısım ıcın guncelleme yapıyor
                context.SaveChanges();
            }
        }
    }
}