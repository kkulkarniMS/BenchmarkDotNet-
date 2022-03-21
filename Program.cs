using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Characteristics;
using BenchmarkDotNet.ConsoleArguments.ListBenchmarks;
using BenchmarkDotNet.Running;
using System;

namespace BenchmarkDotNet 
{
    class Program
    {
        static void Main(string[] args)
        {            
            BenchmarkRunner.Run<AvoidSplit>();
        }        
    }

    [MemoryDiagnoser]
    public class AvoidSplit
    {
        char separator = ',';
        string marketList = "fr-fr,fr-gf,fr-gp,fr-mq,fr-yt,fr-re,nl-nl,ca-es,de-at,fr-be,nl-be,da-dk,fi-fi,de-de,en-ie,it-it,pl-pl,pt-pt,es-es,sv-se,nb-no,de-ch,fr-ch,en-gb,bg-bg,hr-hr,en-cy,cs-cz,et-ee,el-gr,hu-hu,ga-ie,lv-lv,lt-lt,lb-lu,fr-lu,de-lu,mt-mt,ro-ro,ro-md,sl-si,gd-gb,cy-gb,is-is,nn-no,rm-ch,sk-sk,de-li,pt-br";
        string featureList = "allexpusers,encontenprot2,aranscontent,arbimage4,arbnews4,arbvideo4,bigcontthr1,isoththflt2,newcontthr1,smacontthr5,topicnextd,vidcontthr1,d-thshld39,d-thshldspcl40,d-thshld77,d-thshld78,transmainadd,dictorcacfenb,widerl1cf,localunimkt1,wpostampc,locpbafull2,locpbal1,locpbal2,locpbal3v2,locpbal4v2,mqorderfix,gdiadextensionsaacf,foxcf,cert3cf,bingbrand,bfbms3sbot,bfbms3sasync,bfbmsbot,curfixdesktabcf,restbolduxcf,countryhubtf,destgben,extended,usedestv2,preefwticon,asnorm_nmblend_t,addnewsc,asenmodnorm,asnrsuppoff,blendnmsugg,removeemptyprefix,prerx79819,rx80093,rx80095,rcuds52_ct,sngobigdebug,asdesk_c,disimgdedup,mmvidexp28,ceborpad02hotf,jpadexp23,lgncwidcf,idpocrtextint,ocrint,d-thshld42,preeeitabsgmscf,iafiltersc,embedimg15,l2cuw2rc1,muiddflt2cf,igflt11cf,deeplinkmaxrow0tf,arbdlhasuaseabox0,arbhasuaseabox,qnafscolorcf,thpricetrend,arblatrrmmaunitf,arbfmltansbmma,arbrmltanomma,showattrtacf,localmidp,idplocalcard,local,techsimsearch,techsimflight,finpricenoti2,mapsscopecf,mapsadsext1cf,collapsetptf,arbruleclpstp,clpstpanslist,recipeanndo,esrecann,esrecannmode1,recipevserpcf,nosharerv2,disableqsmc,max10child,stackedranking,prg-fin-curr,finrsmax15,vidclicktmc,boplistview,preboldrs3,qbqspillshapetf1,qbqscopyg,algoclickable,fdbknewlayoutcf,v2movwcdb,entv1sup,entv2fact,kcergate,kcv2img,kcv2trigmov,kcv2vid,v1carouselsup,v2prem,phsmallest_cf,bfbsppqryoffcf,msbmstopicnoux,bfbmstopic,bfbmstopicnoux,bfbmspreogcf,bfbntspfcf,bfbtopic,bfbmsstkyftcf,imgdiversityexp4,collagestripc,algounclampall,algounclamp,algounclampsnr,topicdownload,precondintl,trendbelowcf,preloadtrendpane,trendbelowpag,trendbelowpagcf,trendnoiid,trendwithajaxserp,bfprwithigcf,gkf67r6yghj,gbjmgui9,bawbnp,redirf1tf4,helpredir120,helpredirpersist,categorywwv1,vslredesigncr,pagerecodisabled,nopagereco,t2_model_refresh_it4_4,arbiterscale21,arbrev8,arbsbsscale1,arbssrx8,arbtweedierev,disablelowimgdpr,bfcachelogcf,socpostlocaltpcf,localsocialcf,localsocialpost,hpnewtabcf,freshranker1,enhanfactsv1cf,asxapadsdisbot,reposflt1,rcgmm2cr,rcgmsb1,renomove,rsprodcf2,ppiagap10,preprecurcf";
        HashSet<string> marketListSet = new HashSet<string>();
        HashSet<string> featureListSet = new HashSet<string>();
        HashSet<string> marketListSetWithSplit = new HashSet<string>();
        HashSet<string> featureListSetWithSplit = new HashSet<string>();        

        [Benchmark]
        public void UsingParseNext()
        {
            ReadOnlySpan<char> marketListSpan = marketList.AsSpan();
            ReadOnlySpan<char> featureListSpan = featureList.AsSpan();
            ReadOnlySpan<char> temp = null;

            while (!featureListSpan.IsEmpty)
            {
                temp = ParseNext(ref featureListSpan, separator);
                if (temp != ReadOnlySpan<char>.Empty)
                {
                    featureListSet.Add(temp.ToString());
                }
            }
            temp = null;
            while (!marketListSpan.IsEmpty)
            {
                temp = ParseNext(ref marketListSpan, separator);
                if (temp != ReadOnlySpan<char>.Empty)
                {
                    marketListSet.Add(temp.ToString());
                }
            }
        }

        [Benchmark]
        public void UsingSplit()
        {           
            foreach (var fl in featureList.Split(','))
            {
                featureListSetWithSplit.Add(fl.Trim());
            }

            foreach (var mk in marketList.Split(','))
            {
                marketListSetWithSplit.Add(mk.Trim());
            }
        }
        
        public ReadOnlySpan<char> ParseNext(ref ReadOnlySpan<char> originalString, char separator)
        {
            if (originalString.IsEmpty)
            {
                return ReadOnlySpan<char>.Empty;
            }

            var separatorIndex = originalString.IndexOf(separator);

            // originalString = ","
            if (separatorIndex == 0)
            {
                originalString = originalString.Length == 1 ? ReadOnlySpan<char>.Empty : originalString.Slice(1);
                return ReadOnlySpan<char>.Empty;
            }

            /*  if separatorIndex is -1 that means seperator is not found
             *     then originalString will be currentFieldSpan and make originalString empty
             */
            var currentSpanLen = separatorIndex == -1 ? originalString.Length : separatorIndex;
            var currentFieldSpan = originalString.Slice(0, currentSpanLen);
            originalString = originalString.Length <= currentSpanLen + 1 ? ReadOnlySpan<char>.Empty : originalString.Slice(currentSpanLen + 1);

            return currentFieldSpan.Trim();
        }
    }
