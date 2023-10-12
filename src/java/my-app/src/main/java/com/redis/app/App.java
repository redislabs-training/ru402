package com.redis.app;
import redis.clients.jedis.Jedis;
import redis.clients.jedis.UnifiedJedis;
import redis.clients.jedis.search.*;
import redis.clients.jedis.HostAndPort;
import java.util.Map;
import java.util.HashMap;



public class App {

    public static void main(String[] args) {

        //Jedis jedis = new Jedis("localhost", 6379);
        //jedis.set("hello", "world");
        //System.out.println(jedis.get("hello"));

        UnifiedJedis unifiedjedis = new UnifiedJedis(new HostAndPort("localhost", 6379));

        Map<String, Object> attr = new HashMap<>();
        attr.put("TYPE", "FLOAT32");
        attr.put("DIM", 256);
        attr.put("DISTANCE_METRIC", "L2");
        attr.put("INITIAL_CAP", 3);
        Schema schema = new Schema().addHNSWVectorField("profile_image", attr).addNumericField("credit");
        unifiedjedis.ftCreate("vector_idx", IndexOptions.defaultOptions(), schema);
    }
}
